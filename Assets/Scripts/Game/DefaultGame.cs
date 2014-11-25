using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DefaultGame  {

	// Use this for initialization
	public GameStateModel Game;

	public DefaultGame(){
		Game = new GameStateModel();

		Game.ActiveSave = "Default";

		// ---- Flags
		Game.GlobalFlags.Add(new GameFlag{
			Name = "HasOpenedContainer",
			State = false
		});

		// ---- Player Inventory
		var c = new ContainerModel();
		c.Width = 4;
		c.Height = 4;
		c.ContainerId = "Inventory";
		Game.PlayerState = new PlayerStateModel{
			Name = "Asshole",
			Inventory = c
		};

		// ----  Scenes
		SceneModel TestScene = new SceneModel();
		TestScene.Name = "TestScene";

		// ---- // ---- Scene Containers
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

		// ---- NPCs
		// ----// ---- NPC Dialog
		DialogState ds = new DialogState{
			Text = new string[] {"This is how it begins...", "Begins..."}
		};

		DialogState dss = new DialogState{
			Text = new string[] {"Second is ok"}
		};

		DialogState dsss = new DialogState{
			Text = new string[] {"Third... Lucky?? LOL!! L!!!", "..."}
		};
	
		List<DialogItem> d = new List<DialogItem>();
		d.Add (new DialogItem{
			Id = 0,
			Name = "Greetings",
			DialogState = ds,
			Links = new int[]{1,2}.ToList()
		});
		d.Add (new DialogItem{
			Id = 1,
			Name = "Option for 2",
			DialogState = ds,
			Links = new int[]{0,2}.ToList()
		});
		d.Add (new DialogItem{
			Id = 2,
			Name = "Let's move along to 3",
			DialogState = ds,
			Links = new int[]{1}.ToList()
		});

		// ----// ---- NPC Guy
		Game.NPCs.Add (new NPCStateModel{
			Name = "FirstNPC",
			Position = new Vector3(3,3,3),
			Dialog = d
		});

		// ---- Game State Intialization
		Game.Scenes.Add (TestScene);

		Game.Scene = TestScene;

	}
}



