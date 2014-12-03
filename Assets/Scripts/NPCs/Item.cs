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
		var gameManager = (GameManager)GameObject.Find ("GameManager").GetComponent<GameManager>();
		var item = gameManager.Game.Scene.Objects.FirstOrDefault(i => i.Id == State.Id);
		item = State;
	}

	void Destroy(){
		UnityEngine.Object.Destroy(this.gameObject);
	}
}
