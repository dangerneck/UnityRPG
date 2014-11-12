using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateModel
{
		public string ActiveSave;
		public List<GameFlag> GlobalFlags;
		public List<SceneModel> Scenes;
		public PlayerStateModel PlayerState;
		public SceneModel Scene;

		public GameStateModel ()
		{
				GlobalFlags = new List<GameFlag> ();
				Scenes = new List<SceneModel> ();
				PlayerState = new PlayerStateModel ();
				Scene = new SceneModel ();
		}
}
