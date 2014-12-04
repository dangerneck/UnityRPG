using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateModel
{
	public string ActiveSave;
	public List<GameFlag> GlobalFlags;
	public List<SceneModel> Scenes;
	public List<NPCStateModel> NPCs;
	public PlayerStateModel PlayerState;
	public SceneModel Scene;
	public Vector3 TestVector;

	public int Day;
	public float Time;
	public int WeekDay{
		get{
			return Day % 7;
		}
	}

	public GameStateModel ()
	{
			GlobalFlags = new List<GameFlag> ();
			Scenes = new List<SceneModel> ();
			PlayerState = new PlayerStateModel ();
			NPCs = new List<NPCStateModel>();
			Scene = new SceneModel ();
	}
}
