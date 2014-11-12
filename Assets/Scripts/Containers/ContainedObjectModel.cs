
using UnityEngine;
using Newtonsoft.Json;

public class ContainedObjectModel
{
	public Vector3 Position{get;set;}
	public bool Stackable{get;set;}
	public string Id {get;set;}
	public int Stacks{get;set;}
	public string Type;

	public void CreateInWorld(Vector3 position){

	}
	
}
