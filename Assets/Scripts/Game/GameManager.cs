using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System;

public class GameManager : MonoBehaviour {

	string SavePath = Application.dataPath;

	public GameStateModel Game;
	public DefaultGame DGame;
	public Dictionary<string,Texture> LoadedTextures;
	public int ScreenHeight;
	public int ScreenWidth;
	public float ScreenXCenter;
	public float ScreenYCenter;

	int currentHour = -1;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
		Game = new GameStateModel();
		LoadedTextures = new Dictionary<string,Texture>();
		LoadAllTextures();
		ScreenHeight = Screen.height;
		ScreenWidth = Screen.width;
		ScreenXCenter = ScreenWidth / 2;
		ScreenYCenter = ScreenHeight / 2;
		DGame = new DefaultGame();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space")){
			StartGame ("Asshole");
		}
		if (Input.GetKeyDown ("z")){
			StartGame ("Default");
		}
		if (Input.GetKeyDown ("t")){
			SaveGame();
		}

		HandleTime(Time.deltaTime);
	}

	void StartGame(string saveId = "Default")
	{
		if (saveId != "Default"){
			Game = LoadGame(saveId);
			ChangeScene (new ChangeSceneModel{ SceneId = Game.Scene.Name, OnComplete = null});
		}else{
			Game = DGame.Game;
			ChangeScene (new ChangeSceneModel{ SceneId = Game.Scene.Name, OnComplete = null});
		}
	}

	void ChangeScene(ChangeSceneModel m)
	{
		//Set all scheduled task items for those currently moving to their place
		foreach(var npc in Game.NPCs){
			if (npc.Scene == Game.Scene.Name){
				var item = npc.LifetimeSchedule.FirstOrDefault(s => s.Day == Game.Day && s.Hour == Game.Hour);
				if (item == null){
					item = npc.WeeklySchedule.FirstOrDefault(s => s.Weekday == Game.WeekDay && s.Hour == Game.Hour);
				}
				if (item != null){
					npc.Activity = item.Activity;;
					npc.Position = item.Position;
					npc.Scene = item.Scene;
				}
			}
		}

		Game.Scene = Game.Scenes.Where (s => s.Name == m.SceneId).FirstOrDefault();

		Application.LoadLevel (m.SceneId);
		if (m.OnComplete != null){
			m.OnComplete();
		}
	}

	void OnLevelWasLoaded(int index)
	{
		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
		foreach (var c in allObjects){
			if (c.activeInHierarchy){
				var comt = c.GetComponent<Container>();
				var npc = c.GetComponent<NPC>();

				if (comt != null){
					comt.GameManager = this;
					comt.enabled = true;
				}

				// Enable hard-coded npcs
				if (npc != null){
					npc.GameManager = this;
					npc.enabled = true;
				}
			}
		}

		// Instantiate the player
		GameObject playerPrefab = (GameObject)Resources.Load("Prefabs/PlayerPrefab");
		var instance = (GameObject)UnityEngine.Object.Instantiate(playerPrefab, new Vector3(1f,1.5f,-10f), Quaternion.identity);
		var player = instance.GetComponentInChildren<PlayerControl>();
		if (player != null)
		{
			player.GameManager = this;
			player.enabled = true;
			player.State = Game.PlayerState;
		}

		// Instantiate Items
		foreach(var item in Game.Scene.Objects){
			item.CreateInWorld();
		}
		foreach(var npc in Game.NPCs.Where (n => n.Scene == Game.Scene.Name)){
			npc.CreateInScene();
		}
	}

	void LoadAllTextures()
	{
		LoadedTextures.Add ("Placeholder Container", Resources.Load<Texture2D>("Textures/container-placeholder"));
		LoadedTextures.Add ("Container Slot", Resources.Load<Texture2D>("Textures/container-slot"));
		LoadedTextures.Add ("Container Slot Active", Resources.Load<Texture2D>("Textures/container-slot-active"));
		LoadedTextures.Add ("Fresh Ham", Resources.Load<Texture2D>("Textures/fresh-ham"));

	}

	void HandleTime(float t)
	{
		Game.Time += t*5;
		Game.Hour = (int)Game.Time / 60;

		if (Game.Time >= 1440){
			float d = Game.Time - 1440;
			Game.Time = d;
			Game.Day += 1;
			Game.Hour = 0;
		}

		if (Game.Hour != currentHour){
			currentHour = Game.Hour;
			CheckAllAbsentSchedules();
		}
	}

	void SaveGame()
	{
		JsonSerializer jss = new JsonSerializer();
		jss.Converters.Add(new Vector3Converter());
		jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
		string game = "";
		byte[] gameBuffer;

		Game.TestVector = new Vector3(1.0f,2.0f,3.0f);

		if (Game.ActiveSave == "Default"){
			Game.ActiveSave = Game.PlayerState.Name;
		}
		using (FileStream ms = new FileStream(Game.ActiveSave + ".txt", FileMode.Truncate)){
			using (StreamWriter sw = new StreamWriter(ms)){
				using (JsonWriter writer = new JsonTextWriter(sw))		
				{		
					jss.Serialize(writer, Game);	
					sw.Flush();
					ms.Position = 0;
					using (StreamReader sr = new StreamReader(ms)){
						game = sr.ReadToEnd ();
					}
				}
			}
		}
	}

	GameStateModel LoadGame(string name)
	{
		GameStateModel game = new GameStateModel();
		game = (GameStateModel)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(name + ".txt"), typeof(GameStateModel));
		return game;
	}

	void CheckAllAbsentSchedules(bool forceAll = false)
	{
		List<NPCStateModel> absentNpcs = new List<NPCStateModel>();
		if (forceAll){
			absentNpcs = Game.NPCs;
		}else{
			absentNpcs = Game.NPCs.Where (npc => npc.Scene != Game.Scene.Name).ToList ();
		}

		foreach(var npc in absentNpcs){
			var item = npc.LifetimeSchedule.FirstOrDefault(s => s.Day == Game.Day && s.Hour == Game.Hour);
			if (item == null){
				item = npc.WeeklySchedule.FirstOrDefault(s => s.Weekday == Game.WeekDay && s.Hour == Game.Hour);
			}
			if (item != null){
				if (item.Scene == this.Game.Scene.Name){
					Vector3 instantiatePos;
					if (!string.IsNullOrEmpty(item.FromExit)){
						instantiatePos = Game.Scene.Exits.FirstOrDefault (e => e.To == item.FromExit).Position;
					}else{
						instantiatePos = Game.Scene.Exits.FirstOrDefault().Position;
					}
					npc.Scene = this.Game.Scene.Name;
					npc.CreateInScene(instantiatePos);
				}else{
					npc.Activity = item.Activity;;
					npc.Position = item.Position;
					npc.Scene = item.Scene;
				}
			}
		}
	}
}


