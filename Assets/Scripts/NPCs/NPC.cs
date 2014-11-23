using UnityEngine;
using System.Collections;
using System.Linq;

public class NPC : MonoBehaviour {

	public NPCStateModel State{get;set;}
	public string CurrentActivity{get;set;}

	bool DialogOpen = false;
	DialogItem CurrentDialogItem;
	int DialogStateIndex;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// do whatever current activity
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
