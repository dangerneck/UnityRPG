using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneModel {
	public string Name;
	public List<ContainerModel> Containers;
	public List<GameFlag> Flags;
	public List<ContainedObjectModel> Objects;
	public List<SceneExitModel> Exits;

	public SceneModel(){
		Containers = new List<ContainerModel>();
		Flags = new List<GameFlag>();
		Objects = new List<ContainedObjectModel>();
	}

}
