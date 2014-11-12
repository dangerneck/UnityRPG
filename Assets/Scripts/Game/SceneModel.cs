using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneModel {
	public string Name;
	public List<ContainerModel> Containers;
	public List<ContainedObjectModel> NPCs;
	public List<GameFlag> Flags;
	public List<ContainedObjectModel> Objects;

	public SceneModel(){
		Containers = new List<ContainerModel>();
		NPCs = new List<ContainedObjectModel>();
		Flags = new List<GameFlag>();
		Objects = new List<ContainedObjectModel>();
	}

}
