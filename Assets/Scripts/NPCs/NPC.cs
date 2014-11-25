using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class NPC : MonoBehaviour {

	public string Name;

	public NPCStateModel State{get;set;}
	public string CurrentActivity{get;set;}
	public GameManager GameManager{get;set;}

	bool DialogOpen = false;
	DialogItem CurrentDialogItem;
	int DialogStateIndex;
	List<DialogItem> AvailableDialogOptions;

	// Dialog GUI
	float padding = 0.05f;
	float lineHeight;

	Rect textRect;
	Rect continueRect;
	Rect optionsRect;
	Rect contentRect;

	void Start () {

		State = GameManager.Game.NPCs.FirstOrDefault(n => n.Name == Name);

		if (State == null){ enabled = false; }

		CurrentDialogItem = State.Dialog.First (d => d.Id == 0);
		DialogStateIndex = 0;
		AvailableDialogOptions = CurrentDialogItem.Links.Select (i => this.State.Dialog.FirstOrDefault(d => d.Id == i)).ToList ();

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
		if (Input.GetKeyDown ("d")){
			if (!DialogOpen){
				OpenDialog ();
			}else{
				CloseDialog ();
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
				GUI.Label (continueRect, "Continue...");
			}
			GUI.BeginScrollView(optionsRect,new Vector2(0.0f,0.0f),optionsRect);
			int c = 0;
			foreach(var d in AvailableDialogOptions){
				GUI.Label (new Rect(optionsRect.x,optionsRect.y + c * lineHeight, optionsRect.width, lineHeight), d.Name);
				c++;   
			}
			GUI.EndScrollView();
		}
	}

	void OpenDialog()
	{
		DialogOpen = true;
		GameObject.Find ("Player").GetComponent<PlayerControl>().ClaimControlFocus(this.gameObject);
	}
	
	void CloseDialog()
	{
		DialogOpen = false;
		GameObject.Find ("Player").GetComponent<PlayerControl>().ClaimControlFocus();
	}
	
}
