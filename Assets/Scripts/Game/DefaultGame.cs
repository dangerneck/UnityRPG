using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefaultGame  {

	// Use this for initialization
	public GameStateModel Game;

	public DefaultGame(){
		Game = new GameStateModel();

		Game.ActiveSave = "Default";
		Game.GlobalFlags.Add("HasOpenedContainer", false);

		Game.PlayerState = new PlayerStateModel{
			Name = "Asshole"
		};

		SceneModel TestScene = new SceneModel();
		TestScene.Name = "TestScene";

		var t = new List<ContainedObjectModel>();
		t.Add(new ContainedObjectModel{
			Position = new Vector3(0f,0f,0f),
			Stackable = false,
			Type = "Fresh Ham"
		});
		TestScene.Containers.Add (new ContainerModel{
			ContainerId = "First",
			containedObjects = t
		});

		TestScene.Objects.Add (new ContainedObjectModel{
			Position = new Vector3(5f,5f,5f),
			Stackable = false,
			Type = "Fresh Ham"
		});


		Game.Scenes.Add (TestScene);

		Game.Scene = TestScene;

	}
}



