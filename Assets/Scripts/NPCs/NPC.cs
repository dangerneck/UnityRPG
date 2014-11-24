using UnityEngine;
using System.Collections;
using System.Linq;

public class NPC : MonoBehaviour {

	public NPCStateModel State{get;set;}
	public string CurrentActivity{get;set;}
	public GameManager GameManager{get;set;}

	bool DialogOpen = false;
	DialogItem CurrentDialogItem;
	int DialogStateIndex;

	// Dialog GUI
	float padding = 0.1f;
	float lineHeight;
	float optionsHeight;

	Rect textRect;
	Rect continueRect;
	Rect optionsRect;
	Rect contentRect;

	// Use this for initialization
	void Start () {
		var sw = GameManager.ScreenWidth;
		var sh = GameManager.ScreenHeight;
		var scx = GameManager.ScreenXCenter;
		var scy = GameManager.ScreenYCenter;

		var bw = sw * (1 - padding*2);
		var bh = sh * (1 - padding*2);

		contentRect = new Rect(sw * padding,sh * padding,bw,bh);

		lineHeight = contentRect.height * padding;

		textRect = new Rect(contentRect.x + contentRect.width * padding, contentRect.y + contentRect.height * padding, contentRect.width*(1-padding*2), contentRect.height*(1-padding*2)*0.3f);
		continueRect = new Rect(textRect.x, textRect.yMax + lineHeight, textRect.width, lineHeight * 1);
		optionsRect = new Rect(continueRect.x, continueRect.yMax + lineHeight, textRect.width, textRect.height);
	}
	
	// Update is called once per frame
	void Update () {
		// do whatever current activity

	}

	void OnGui(){
		if (DialogOpen){
			GUI.Box (contentRect, "Dialog");
			GUI.Box (textRect, "Content");
			GUI.Box (continueRect, "Continue...");
			GUI.Box (optionsRect, "Options...");
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
