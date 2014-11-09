using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Container : MonoBehaviour
{
	public GameManager GameManager;
	public List<ContainedObjectModel> containedObjects;
	public string ContainerId;
	public Texture Gump;
	public float GumpPadding;
	public int Width;
	public int Height;

	bool Opened;
	int Pointer = 0;


	// Use this for initialization
	void Start ()
	{
		var loadedContainer = FindObjectOfType<GameManager>().Game.Scene.Containers.Where(c => c.ContainerId == ContainerId).FirstOrDefault();
		containedObjects = loadedContainer.containedObjects;
		GumpPadding = loadedContainer.GumpPadding;
		Width = loadedContainer.Width;
		Height = loadedContainer.Height;
	}

	// Update is called once per frame
	void Update ()
	{
		if (!Opened){
			if (Input.GetMouseButton(0)){
				var maincam = GameObject.Find ("Main Camera");
				Ray ray = maincam.camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hits;
				hits = Physics.RaycastAll (ray);
				foreach (var h in hits){
					if (h.transform == transform){
						Open ();
					}
				}
			}
		}else{
			var playerControl = (PlayerControl)GameObject.Find ("Player").GetComponent<PlayerControl>();
			if (playerControl.controlFocus == this.gameObject)
			{
				if (Input.GetKeyDown("left")){
					Pointer--;
				}
				if (Input.GetKeyDown("right")){
					Pointer++;
				}
				if (Input.GetKeyDown("up")){
					Pointer -= Width;
				}
				if (Input.GetKeyDown("down")){
					Pointer += Width;
				}

				if (Input.GetKeyDown("z")){
					var o = containedObjects.ElementAt(Pointer);
					if (o != null){
						playerControl.inventory.containedObjects.Add(o);
						containedObjects.Remove (o);
					}
				}

				if (Input.GetKeyDown("x")){
					var o = containedObjects.ElementAt(Pointer);
					if (o != null){
						o.CreateInWorld(playerControl.gameObject.transform.position);
						containedObjects.Remove (o);
					}
				}

				if (Pointer < 0){ Pointer = 0; }
				if (Pointer > Width * Height - 1){ Pointer = Width * Height - 1; }

				if (Input.GetKeyDown("c")){
					Close ();
				}

			}
		}
	}

	void OnGUI()
	{
		if (Opened)
		{
			float wCenter = Screen.width/2;
			float hCenter = Screen.height/2;
			float cx = wCenter - Gump.width/2;
			float cy = hCenter - Gump.height/2;
			int centreW = Width/2;
			int centreH = Height/2;
			GUI.DrawTexture(new Rect(cx, cy, Gump.width, Gump.height), Gump);
			var textures = GameManager.LoadedTextures;

			for(int j = 0; j < Width; j++){
				for (int i = 0; i < Height; i++){
					float spaceX = wCenter - ((centreW - i) * 32);
					float spaceY = hCenter - ((centreH - j) * 32);
					bool active = i +(j*Width) == Pointer;
					GUI.DrawTexture(new Rect(spaceX, spaceY, 32, 32), active ? textures["Container Slot Active"]: textures["Container Slot"]);
					if (containedObjects.Count >= (i+j*Width)+1){
						if (containedObjects.ElementAt (i+j*Width) != null){
							GUI.DrawTexture(new Rect(spaceX, spaceY, 32, 32), textures[containedObjects.ElementAt (i+j*Width).Type]);
						}
					}
				}
			}
			if (GUI.Button(new Rect(cx, cy - 20, 20, 20), "x")){
				Close ();
			}
		}
	}

	void Open()
	{
		Opened = true;
		GameObject.Find ("Player").GetComponent<PlayerControl>().ClaimControlFocus(this.gameObject);
	}

	void Close()
	{
		GameManager.Game.Scene.Containers.Where (c => c.ContainerId == ContainerId).FirstOrDefault().containedObjects = containedObjects;
		Opened = false;
		GameObject.Find ("Player").GetComponent<PlayerControl>().ClaimControlFocus();
	}

	void OnMouseDown()
	{
		if (!Opened){
			Open ();
		}
	}

	void UpdateContainer()
	{

	}
}
