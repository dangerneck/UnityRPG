using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DialogItem {
	public int Id{get;set;}
	public string Name{get;set;}
	public GameFlag RequisiteFlag{get;set;}
	public DialogState DialogState{get;set;}
	public Action OnSelect{get;set;}
	public List<int> Links{get;set;}
}
