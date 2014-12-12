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
			ChangeScene (Game.Scene.Name);
		}else{
			Game = DGame.Game;
			ChangeScene (Game.Scene.Name);
		}
	}

	void ChangeScene(string sceneId)
	{
		var sceneRef = Game.Scenes.Where(s => s.Name == Game.Scene.Name).FirstOrDefault();
		Game.Scenes.Add (Game.Scene);
		Game.Scenes.Remove (sceneRef);
		Game.Scene = Game.Scenes.Where (s => s.Name == sceneId).FirstOrDefault();
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
		this.Game.Time += t;
		if (this.Game.Time >= 1440){
			float d = this.Game.Time - 1440;
			this.Game.Time = d;
			this.Game.Day += 1;
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
}


