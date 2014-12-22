using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Container : MonoBehaviour
{
	public GameManager GameManager;
	public ContainerModel State;
	public string ContainerId;
	public Texture Gump;
	public float GumpPadding;

	float cx;
	float cy;
	int centreW;
	int centreH;

	bool Opened;
	int Pointer = 0;


	// Use this for initialization
	void Start ()
	{
		var loadedContainer = FindObjectOfType<GameManager>().Game.Scene.Containers.FirstOrDefault(c => c.ContainerId == ContainerId);
		State = loadedContainer; 

		cx = GameManager.ScreenXCenter - Gump.width/2;
		cy = GameManager.ScreenYCenter - Gump.height/2;
		centreW = State.Width/2;
		centreH = State.Height/2;
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
					Pointer -= State.Width;
				}
				if (Input.GetKeyDown("down")){
					Pointer += State.Width;
				}

				if (Input.GetKeyDown("z")){
					var o = State.containedObjects.ElementAt(Pointer);
					if (o != null){
						if (!playerControl.State.Inventory.IsFull){
							if (playerControl.State.Inventory.Add(o)){
								State.containedObjects.Remove (o);
							}
						}
					}
				}

				if (Input.GetKeyDown("x")){
					var o = State.containedObjects.ElementAt(Pointer);
					if (o != null){
						o.CreateInWorld(new Vector3(playerControl.gameObject.transform.position.x, playerControl.gameObject.transform.position.y - 0.5f, playerControl.gameObject.transform.position.z));
						State.containedObjects.Remove (o);
					}
				}

				if (Pointer < 0){ Pointer = 0; }
				if (Pointer > State.Width * State.Height - 1){ Pointer = State.Width * State.Height - 1; }

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

			GUI.DrawTexture(new Rect(cx, cy, Gump.width, Gump.height), Gump);
			var textures = GameManager.LoadedTextures;

			for(int j = 0; j < State.Width; j++){
				for (int i = 0; i < State.Height; i++){
					float spaceX = GameManager.ScreenXCenter - ((centreW - i) * 32);
					float spaceY = GameManager.ScreenYCenter - ((centreH - j) * 32);
					bool active = i +(j*State.Width) == Pointer;
					GUI.DrawTexture(new Rect(spaceX, spaceY, 32, 32), active ? textures["Container Slot Active"]: textures["Container Slot"]);
					if (State.containedObjects.Count >= (i+j*State.Width)+1){
						if (State.containedObjects.ElementAt (i+j*State.Width) != null){
							GUI.DrawTexture(new Rect(spaceX, spaceY, 32, 32), textures[State.containedObjects.ElementAt (i+j*State.Width).Type]);
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
		GameManager.Game.Scene.Containers.Where (c => c.ContainerId == ContainerId).FirstOrDefault().containedObjects = State.containedObjects;
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
