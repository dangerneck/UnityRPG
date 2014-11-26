using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NPCStateModel {
	public string Name{get;set;}
	public Vector3 Position{get;set;}
	public int InitialDialogOptionId{get;set;}
	public List<GameFlag> Flags{get;set;}

	[JsonIgnore]
	public List<ScheduleItem> WeeklySchedule{get;set;}
	[JsonIgnore]
	public List<ScheduleItem> LifetimeSchedule{get;set;}
	[JsonIgnore]
	public List<DialogItem> Dialog{get;set;}
	// model
	// animator
	// dialog model

}
