using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ContainerModel
{
	public List<ContainedObjectModel> containedObjects;
	public string ContainerId;
	public Texture Gump;
	public float GumpPadding;
	public int Width;
	public int Height;

	public ContainerModel(){
		containedObjects = new List<ContainedObjectModel>();
	}
}
