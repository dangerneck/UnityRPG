using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneModel {
	public string Name;
	public List<ContainerModel> Containers;
	public List<NPCStateModel> NPCs;
	public List<GameFlag> Flags;
	public List<ContainedObjectModel> Objects;
	public List<SceneExitModel> Exits;

	public SceneModel(){
		Containers = new List<ContainerModel>();
		NPCs = new List<NPCStateModel>();
		Flags = new List<GameFlag>();
		Objects = new List<ContainedObjectModel>();
	}

}
