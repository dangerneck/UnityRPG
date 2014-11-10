using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ContainerModel
{
	public List<ContainedObjectModel> containedObjects;
	public string ContainerId;
	public int Width;
	public int Height;

	public ContainerModel(){
		containedObjects = new List<ContainedObjectModel>();
	}

	public bool IsFull{
		get{
			return (containedObjects.Count == Width * Height);
		}
	}

	public bool IsEmpty{
		get{
			return (containedObjects.Count == 0);
		}
	}

	public bool Add(ContainedObjectModel c){
		if (!IsFull){
			containedObjects.Add (c);
			return true;
		}
		return false;
	}

	public bool Remove(ContainedObjectModel c){
		if (containedObjects.Contains (c)){
			containedObjects.Remove (c);
			return true;
		}
		return false;
	}
		
}
