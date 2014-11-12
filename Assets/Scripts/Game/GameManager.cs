using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour {

	string SavePath = Application.dataPath;

	public GameStateModel Game;
	public Dictionary<string,Texture> LoadedTextures;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
		Game = new GameStateModel();
		LoadedTextures = new Dictionary<string,Texture>();
		LoadAllTextures();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space")){
			StartGame ("Asshole");
		}
		if (Input.GetKeyDown ("t")){
			SaveGame();
		}
	}

	void StartGame(string saveId = "Default")
	{
		if (saveId != "Default"){
			string saveString = System.IO.File.ReadAllText(saveId + ".txt");
			Game = (GameStateModel)JavaScriptConvert.DeserializeObject(saveString, typeof(GameStateModel));
			ChangeScene (Game.Scene.Name);
		}else{
			var d = new DefaultGame();
			Game = d.Game;
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
				var comp = c.GetComponent<Container>();
				if (comp != null){
					comp.GameManager = this;
					comp.enabled = true;
				}
				// Instead we should instantiate the player prefab but let's just do this for now TODO
				var player = c.GetComponent<PlayerControl>();
				if (player != null)
				{
					player.GameManager = this;
					player.enabled = true;
					player.State = Game.PlayerState;
				}
			}
		}
	}

	void LoadAllTextures()
	{
		LoadedTextures.Add ("Placeholder Container", Resources.Load<Texture2D>("Textures/container-placeholder"));
		LoadedTextures.Add ("Container Slot", Resources.Load<Texture2D>("Textures/container-slot"));
		LoadedTextures.Add ("Container Slot Active", Resources.Load<Texture2D>("Textures/container-slot-active"));
		LoadedTextures.Add ("Fresh Ham", Resources.Load<Texture2D>("Textures/fresh-ham"));

	}

	void SaveGame()
	{
		JsonSerializer jss = new JsonSerializer();
		jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string game = "";
		byte[] gameBuffer;

		if (Game.ActiveSave == "Default"){
			Game.ActiveSave = Game.PlayerState.Name;
		}
		using (FileStream ms = new FileStream(Game.ActiveSave + ".txt", FileMode.OpenOrCreate)){
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

	void LoadGame()
	{

	}
}


