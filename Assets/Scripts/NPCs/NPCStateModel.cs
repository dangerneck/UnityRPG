using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NPCStateModel {
	public string Name{get;set;}
	public Vector3 Position{get;set;}
	public List<ScheduleItem> WeeklySchedule{get;set;}
	public List<ScheduleItem> LifetimeSchedule{get;set;}
	public List<GameFlag> Flags{get;set;}
	public List<DialogItem> Dialog{get;set;}
	// model
	// animator
	// dialog model

}
