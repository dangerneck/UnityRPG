using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
			StartGame ();
		}
	}

	void StartGame(string saveId = "default")
	{
		if (saveId != "default"){
			string saveString = System.IO.File.ReadAllText(SavePath + Path.DirectorySeparatorChar + "Save" + Path.DirectorySeparatorChar + saveId);
			Game = (GameStateModel)Newtonsoft.Json.JavaScriptConvert.DeserializeObject(saveString, typeof(GameStateModel));
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
			}
		}
	}

	void LoadAllTextures()
	{
		LoadedTextures.Add ("Container Slot", Resources.Load<Texture2D>("Textures/container-slot"));
		LoadedTextures.Add ("Container Slot Active", Resources.Load<Texture2D>("Textures/container-slot-active"));
		LoadedTextures.Add ("Fresh Ham", Resources.Load<Texture2D>("Textures/fresh-ham"));
	}
}
