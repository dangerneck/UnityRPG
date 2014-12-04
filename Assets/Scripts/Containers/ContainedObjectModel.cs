
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ContainedObjectModel
{
	public Vector3 Position{get;set;}
	public bool Stackable{get;set;}
	public string ItemId {get;set;}
	public int Stacks{get;set;}
	public string Type;

	public void CreateInWorld(){
		GameObject prefab = (GameObject)Resources.Load("Prefabs/Item");
		var instance = (GameObject)UnityEngine.Object.Instantiate(prefab, this.Position, Quaternion.identity);
		var itemInstance = instance.GetComponent<Item>();
		itemInstance.State = this;
	}
	public void CreateInWorld(Vector3 position){
		this.Position = position;
		this.CreateInWorld();
	}
	
}
