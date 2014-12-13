using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using Pathfinding;

public class NPC : MonoBehaviour {

	public string Name;

	public NPCStateModel State{get;set;}
	public GameManager GameManager{get;set;}

	bool DialogOpen = false;
	DialogItem CurrentDialogItem;
	int DialogStateIndex;
	List<DialogItem> AvailableDialogOptions;

	ScheduleItem currentScheduleItem;

	// Dialog GUI
	float padding = 0.05f;
	float lineHeight;

	Rect textRect;
	Rect continueRect;
	Rect optionsRect;
	Rect contentRect;

	Seeker seeker;
	float walkSpeed = 3f;
	Path currentPath;
	int currentWaypoint = 0;
	float nextWaypointDistance = 1f;
	float moveLerper = 0.0f;
	float accelerationPercent = 0.2f;
	Vector3 moveDirection;
	Vector3 dir;
	Vector3 normDir;
	float npcSize = 1.0f;
	float moveMagnitude;
	int currentHour = -1;

	void Start () {

		State = GameManager.Game.NPCs.FirstOrDefault(n => n.Name == Name);
		seeker = GetComponent<Seeker>();
		var dnpc = GameManager.DGame.Game.NPCs.FirstOrDefault(n => n.Name == Name);
		State.LifetimeSchedule = dnpc.LifetimeSchedule;
		State.WeeklySchedule = dnpc.WeeklySchedule;
		State.Dialog = dnpc.Dialog;

		if (State == null){ enabled = false; }

		var sw = GameManager.ScreenWidth;
		var sh = GameManager.ScreenHeight;
		var scx = GameManager.ScreenXCenter;
		var scy = GameManager.ScreenYCenter;

		var bw = sw * (1 - padding*2);
		var bh = sh * (1 - padding*2);

		contentRect = new Rect(sw * padding,sh * padding,bw,bh);

		lineHeight = contentRect.height * padding;

		textRect = new Rect(contentRect.x + contentRect.width * padding, contentRect.y + contentRect.height * padding, contentRect.width*(1-padding*2), contentRect.height*(1-padding*2)*0.4f);
		continueRect = new Rect(textRect.x, textRect.yMax + lineHeight, textRect.width, lineHeight * 1);
		optionsRect = new Rect(continueRect.x, continueRect.yMax + lineHeight, textRect.width, textRect.height);
	}

	void Update () {
		// Dummy demo dialog dickhole
		if (Input.GetKeyDown ("d")){
			if (!DialogOpen){
				OpenDialog ();
			}else{
				CloseDialog ();
			}
		}

		// Dummy start path toward player
		if (Input.GetKeyDown("p")){
			var player = GameObject.Find ("Player");
			currentScheduleItem = new ScheduleItem{
				Activity = "Wait",
				Scene = GameManager.Game.Scene.Name
			};
			seeker.StartPath (this.transform.position, player.transform.position, OnScheduledPathReady);
		}

		CheckSchedule();

		if (currentPath != null){
			if (currentWaypoint >= currentPath.vectorPath.Count) {
				currentPath = null;
				currentWaypoint = 0;
				OnScheduledPathComplete();
				return;
			}
			
			//Direction to the next waypoint
			dir = (currentPath.vectorPath[currentWaypoint]-transform.position).normalized;

			// Handle move acceleration through lerp
			if (moveLerper != 1)
			{
				if (moveLerper < 1){
					moveLerper += accelerationPercent;
				}
			}

			if (Vector3.Distance (transform.position,currentPath.vectorPath[currentWaypoint]) < nextWaypointDistance) {
				currentWaypoint++;
			}
		}else{
			dir = new Vector3(0,0,0).normalized;
			if (moveLerper != 0)
			{
				moveLerper = 0;
			}
		}

		moveDirection = new Vector3(Mathf.Lerp (0, dir.x * walkSpeed * Time.deltaTime, moveLerper), 0, Mathf.Lerp (0, dir.z * walkSpeed * Time.deltaTime, moveLerper));
		
		// Collision detection for move
		moveMagnitude = moveDirection.magnitude;
		normDir = moveDirection / moveMagnitude;

		RaycastHit rayHit;
		if (!Physics.SphereCast (transform.position, npcSize/2, normDir, out rayHit, moveMagnitude)) {
			transform.position = transform.position + moveDirection;
		}else if (!Physics.SphereCast (transform.position + new Vector3(0f, 0.5f, 0f), npcSize/2, normDir, out rayHit, moveMagnitude)) {
			transform.position = transform.position + moveDirection + new Vector3(0f, 0.5f, 0f);
		}

		//Gravity collision check & move
		moveDirection = new Vector3(0f,-0.1f,0f);
		moveMagnitude = moveDirection.magnitude;
		RaycastHit gravRayHit;
		if (!Physics.SphereCast (transform.position, npcSize/2, Vector3.down, out gravRayHit, moveMagnitude)) {
			transform.position = transform.position + moveDirection;
		}
	}

	void OnGUI(){
		if (DialogOpen){
			GUI.Box (contentRect, "");
			GUI.Box (textRect, "");
			GUI.Label (textRect, CurrentDialogItem.DialogState.Text[DialogStateIndex]);
			if (CurrentDialogItem.DialogState.Text.Length > DialogStateIndex+1){
				GUI.Box (continueRect, "");
				if (GUI.Button (continueRect, "Continue..."))
				{
					DialogStateIndex++;
				}
			}
			GUI.BeginScrollView(optionsRect,new Vector2(0.0f,0.0f),optionsRect);
			int c = 0;
			foreach(var d in AvailableDialogOptions){
				if (GUI.Button(new Rect(optionsRect.x,optionsRect.y + c * lineHeight, optionsRect.width, lineHeight), d.Name)){
					ChangeDialogItem(d);
				}
				c++;   
			}
			if (GUI.Button(new Rect(optionsRect.x,optionsRect.y + c * lineHeight, optionsRect.width, lineHeight), "Bye")){
				CloseDialog ();
			}
			GUI.EndScrollView();
		}
	}

	void ChangeDialogItem(DialogItem d){
		DialogStateIndex = 0;
		this.CurrentDialogItem = d;
		AvailableDialogOptions = CurrentDialogItem.Links.Select (i => this.State.Dialog.FirstOrDefault(di => di.Id == i)).ToList ();
		if (d.OnSelect != null){
			d.OnSelect ();
		}
	}

	void OpenDialog()
	{
		ChangeDialogItem(State.Dialog.FirstOrDefault(x => x.Id == State.InitialDialogOptionId));
		DialogOpen = true;
		GameObject.Find ("Player").GetComponent<PlayerControl>().ClaimControlFocus(this.gameObject);
	}
	
	void CloseDialog()
	{
		DialogOpen = false;
		ChangeDialogItem(State.Dialog.First (d => d.Id == 0));
		GameObject.Find ("Player").GetComponent<PlayerControl>().ClaimControlFocus();
	}

	void CheckSchedule(){
		if (GameManager.Game.Hour != currentHour){
			currentHour = GameManager.Game.Hour;

			var h = GameManager.Game.Hour;
			var d = GameManager.Game.Day;
			var wd = GameManager.Game.WeekDay;

			var item = State.LifetimeSchedule.FirstOrDefault(s => s.Day == d && s.Hour == h);
			if (item == null){
				item = State.WeeklySchedule.FirstOrDefault(s => s.Weekday == wd && s.Hour == h);
			}
			if (item != null){
				currentScheduleItem = item;
				if (currentScheduleItem.Scene == GameManager.Game.Scene.Name){
					seeker.StartPath (transform.position, currentScheduleItem.Position, OnScheduledPathReady);
				}else{
					var exit = GameManager.Game.Scene.Exits.FirstOrDefault(e => e.To == currentScheduleItem.Scene);
					if (exit != null){
						seeker.StartPath(transform.position, exit.Position, OnScheduledPathReady);
					}else{
						seeker.StartPath (transform.position, GameManager.Game.Scene.Exits.FirstOrDefault().Position, OnScheduledPathReady);
					}
				}
			}
		}

	}

	void OnScheduledPathReady(Path p)
	{
		currentPath = p;
		currentWaypoint = 0;
	}

	void OnScheduledPathComplete()
	{
		State.Scene = currentScheduleItem.Scene;
		State.Position = currentScheduleItem.Position;
		State.Activity = currentScheduleItem.Activity;
		SaveStateToGameManager();

		if (currentScheduleItem.Scene != GameManager.Game.Scene.Name){
			UnityEngine.Object.Destroy(this.gameObject);
		}else{
			currentWaypoint = 0;
		}
	}

	void SaveStateToGameManager(){
		var me = GameManager.Game.NPCs.FirstOrDefault(npc => npc.Name == this.Name);
		me = State;
	}

}
