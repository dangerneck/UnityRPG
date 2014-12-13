using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

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

		TestScene.Exits = new List<SceneExitModel>();
		TestScene.Exits.Add (new SceneExitModel{
			Position = new Vector3(14,3,-13),
			To = "SceneTwo"
		});

		// ---- // ---- Scene Containers
		var t = new List<ContainedObjectModel>();
		t.Add(new ContainedObjectModel{
			Position = new Vector3(1f,0f,0f),
			Stackable = false,
			Type = "Fresh Ham",
			ItemId = Guid.NewGuid().ToString()
		});
		TestScene.Containers.Add (new ContainerModel{
			ContainerId = "First",
			containedObjects = t,
			Width = 4,
			Height = 4,
		});

		// ---- // ---- Scene Items
		TestScene.Objects.Add (new ContainedObjectModel{
			Position = new Vector3(5f,0.51f,5f),
			Stackable = false,
			Type = "Fresh Ham",
			ItemId = Guid.NewGuid().ToString()
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
			DialogState = dss,
			Links = new int[]{0,2}.ToList(),
			OnSelect = () => Debug.Log ("This is a delegate passed into the guy!")
		});
		d.Add (new DialogItem{
			Id = 2,
			Name = "Let's move along to 3",
			DialogState = dsss,
			Links = new int[]{1}.ToList()
		});


		List<ScheduleItem> sch = new List<ScheduleItem>();
		sch.Add(new ScheduleItem{
			Day = 0,
			Weekday = 0,
			Hour = 0,
			Activity = "Stand",
			Scene = "TestScene",
			Position = new Vector3(7,2,-12)
		});
		sch.Add(new ScheduleItem{
			Day = 0,
			Weekday = 0,
			Hour = 1,
			Activity = "Stand",
			Scene = "TestScene",
			Position = new Vector3(-3,7,0)
		});
		sch.Add (new ScheduleItem{
			Day = 0,
			Weekday = 0,
			Hour = 2,
			Activity = "Stand",
			Scene = "SceneTwo",
			Position = new Vector3(1,1,1)
		});
		sch.Add (new ScheduleItem{
			Day = 0,
			Weekday = 0,
			Hour = 3,
			Activity = "Stand",
			Scene = "TestScene",
			Position = new Vector3(1,1,1),
			FromExit = "SceneTwo"
		});

		// ----// ---- NPC Guy
		Game.NPCs.Add (new NPCStateModel{
			Name = "FirstNPC",
			Position = new Vector3(3,3,3),
			Dialog = d,
			InitialDialogOptionId = 0,
			WeeklySchedule = sch,
			LifetimeSchedule = new List<ScheduleItem>(),
			Scene = "TestScene"
		});

		// ---- Game State Intialization
		Game.Scenes.Add (TestScene);

		Game.Scene = TestScene;

	}
}



