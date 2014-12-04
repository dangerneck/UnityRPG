using UnityEngine;
using System.Collections;
using System.Linq;

public class Item : MonoBehaviour {

	public ContainedObjectModel State{get;set;}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void UpdateState(){
		var gm = (GameManager)GameObject.Find ("GameManager").GetComponent<GameManager>();
		var item = gm.Game.Scene.Objects.FirstOrDefault(i => i.ItemId == State.ItemId);
		item = State;
	}

	void Destroy(){
		var gm = (GameManager)GameObject.Find ("GameManager").GetComponent<GameManager>();
		var i = gm.Game.Scene.Objects.FirstOrDefault(o => o.ItemId == State.ItemId);
		gm.Game.Scene.Objects.Remove(i);
		UnityEngine.Object.Destroy(this.gameObject);
	}
}
