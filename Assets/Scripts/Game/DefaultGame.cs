using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefaultGame  {

	// Use this for initialization
	public GameStateModel Game;

	public DefaultGame(){
		Game = new GameStateModel();

		Game.ActiveSave = "Default";
		Game.GlobalFlags.Add(new GameFlag{
			Name = "HasOpenedContainer",
			State = false
		});

		var c = new ContainerModel();
		c.Width = 4;
		c.Height = 4;
		c.ContainerId = "Inventory";
		Game.PlayerState = new PlayerStateModel{
			Name = "Asshole",
			Inventory = c
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
			containedObjects = t,
			Width = 4,
			Height = 4,
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



