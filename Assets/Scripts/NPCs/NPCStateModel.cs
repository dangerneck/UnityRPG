using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class NPCStateModel {
	public string Name{get;set;}
	public Vector3 Position{get;set;}
	public int InitialDialogOptionId{get;set;}
	public List<GameFlag> Flags{get;set;}
	public string Scene{get;set;}
	public string Activity{get;set;}

	[JsonIgnore]
	public List<ScheduleItem> WeeklySchedule{get;set;}
	[JsonIgnore]
	public List<ScheduleItem> LifetimeSchedule{get;set;}
	[JsonIgnore]
	public List<DialogItem> Dialog{get;set;}
	// model
	// animator
	// dialog model
	public void CreateInScene(Vector3 instantiatePos){
		GameObject prefab = (GameObject)Resources.Load("Prefabs/NPC");
		var instance = (GameObject)UnityEngine.Object.Instantiate(prefab, instantiatePos, Quaternion.identity);
		var npcInstanceState = instance.GetComponent<NPC>();
		npcInstanceState.State = this;
		npcInstanceState.GameManager = UnityEngine.GameObject.FindObjectOfType<GameManager>();
		npcInstanceState.enabled = true;
	}
	public void CreateInScene(){
		GameObject prefab = (GameObject)Resources.Load("Prefabs/NPC");
		var instance = (GameObject)UnityEngine.Object.Instantiate(prefab, this.Position, Quaternion.identity);
		var npcInstanceState = instance.GetComponent<NPC>();
		npcInstanceState.State = this;
		npcInstanceState.TeleportToScheduleItem = true;
		npcInstanceState.GameManager = UnityEngine.GameObject.FindObjectOfType<GameManager>();
		npcInstanceState.enabled = true;		
	}
}
