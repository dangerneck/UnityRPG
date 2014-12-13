using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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
			StartGame ("Default");
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
			ChangeScene (Game.Scene.Name);
		}else{
			Game = DGame.Game;
			ChangeScene (Game.Scene.Name);
		}
	}

	void ChangeScene(string sceneId)
	{
		var sceneRef = Game.Scenes.Where(s => s.Name == Game.Scene.Name).FirstOrDefault();
		sceneRef = Game.Scene;
		Game.Scene = Game.Scenes.Where (s => s.Name == sceneId).FirstOrDefault();
		Game.Scene.NPCs = Game.NPCs.Where (npc => npc.Scene == Game.Scene.Name).ToList ();
		Application.LoadLevel(sceneId);
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
				// Instead we should instantiate the player prefab but let's just do this for now TODO
				var player = c.GetComponent<PlayerControl>();
				if (player != null)
				{
					player.GameManager = this;
					player.enabled = true;
					player.State = Game.PlayerState;
				}

				// This is where you'd create NPCs in the current scene.
				if (npc != null){
					npc.GameManager = this;
					npc.enabled = true;
				}
			}
		}

		// Instantiate Items
		foreach(var item in Game.Scene.Objects){
			item.CreateInWorld();
		}

		// TODO: Instantiate NPCs
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

	void CheckAllAbsentSchedules()
	{
		var npcs = Game.NPCs.Where (npc => npc.Scene != Game.Scene.Name).ToList ();
		foreach(var npc in npcs){
			var item = npc.LifetimeSchedule.FirstOrDefault(s => s.Day == Game.Day && s.Hour == Game.Hour);
			if (item == null){
				item = npc.WeeklySchedule.FirstOrDefault(s => s.Weekday == Game.WeekDay && s.Hour == Game.Hour);
			}
			if (item.Scene == this.Game.Scene.Name){
				Vector3 instantiatePos;
				if (!string.IsNullOrEmpty(item.FromExit)){
					instantiatePos = Game.Scene.Exits.FirstOrDefault (e => e.To == item.FromExit).Position;
				}else{
					instantiatePos = Game.Scene.Exits.FirstOrDefault().Position;
				}
				GameObject prefab = (GameObject)Resources.Load("Prefabs/NPC");
				var instance = (GameObject)UnityEngine.Object.Instantiate(prefab, instantiatePos, Quaternion.identity);
				var npcInstanceState = instance.GetComponent<NPC>();
				npcInstanceState.State = npc;
				npcInstanceState.GameManager = this;
				npcInstanceState.enabled = true;
			}else{
				npc.Activity = item.Activity;;
				npc.Position = item.Position;
				npc.Scene = item.Scene;
			}
		}
	}
}


