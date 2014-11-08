using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateModel
{
		public string ActiveSave;
		public Dictionary<string,bool> GlobalFlags;
		public List<SceneModel> Scenes;
		public PlayerStateModel PlayerState;
		public SceneModel Scene;

		public GameStateModel ()
		{
				GlobalFlags = new Dictionary<string,bool> ();
				Scenes = new List<SceneModel> ();
				PlayerState = new PlayerStateModel ();
				Scene = new SceneModel ();
		}
}
