using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using Pathfinding;

public class NPC : MonoBehaviour {

	public string Name;

	public NPCStateModel State{get;set;}
	public string CurrentActivity{get;set;}
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
	float walkSpeed = 3.0f;
	Path currentPath;
	int currentWaypoint = 0;
	float nextWaypointDistance = 1f;

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

		if (Input.GetKeyDown("p")){
			var player = GameObject.Find ("Player");
			currentScheduleItem = new ScheduleItem{
				Activity = "Wait",
				Scene = GameManager.Game.Scene.Name
			};
			seeker.StartPath (this.transform.position, player.transform.position, OnScheduledPathReady);
		}

		//CheckSchedule();

		if (currentPath != null){
			if (currentWaypoint >= currentPath.vectorPath.Count) {
				currentPath = null;
				currentWaypoint = 0;
				OnScheduledPathComplete();
				return;
			}
			
			//Direction to the next waypoint
			Vector3 dir = (currentPath.vectorPath[currentWaypoint]-transform.position).normalized;
			dir *= walkSpeed * Time.fixedDeltaTime;
			this.transform.Translate (dir);
			
			//Check if we are close enough to the next waypoint
			//If we are, proceed to follow the next waypoint
			if (Vector3.Distance (transform.position,currentPath.vectorPath[currentWaypoint]) < nextWaypointDistance) {
				currentWaypoint++;
			}
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
		var t = (int)GameManager.Game.Time / 60;
		var d = GameManager.Game.Day;
		var wd = GameManager.Game.WeekDay;

		var item = this.State.WeeklySchedule.FirstOrDefault(s => s.Weekday == wd && s.Time == t);
		if (item != null){
			currentScheduleItem = item;
			if (currentScheduleItem.Scene == GameManager.Game.Scene.Name){
				seeker.StartPath (this.transform.position, currentScheduleItem.Position, OnScheduledPathReady);
			}else{
				// go to the appropriate exit!
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
		this.CurrentActivity = currentScheduleItem.Activity;
	}
}
