using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Container : MonoBehaviour
{

	public List<ContainedObjectModel> containedObjects;
	public string ContainerId;
	public Texture Gump;
	public float GumpPadding;
	bool Opened;

	// Use this for initialization
	void Start ()
	{
		containedObjects = FindObjectOfType<GameManager>().Game.Scene.Containers.Where(c => c.ContainerId == ContainerId).FirstOrDefault().containedObjects;
	}

	// Update is called once per frame
	void Update ()
	{
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
	}

	void OnGUI()
	{
		if (Opened)
		{
			float cx = Screen.width/2 - Gump.width/2;
			float cy = Screen.height/2 - Gump.height/2;
			GUI.DrawTexture(new Rect(cx, cy, Gump.width, Gump.height), Gump);
			foreach(var c in containedObjects){
				GUI.Label(new Rect(cx + GumpPadding + c.Position.x, cy + GumpPadding + c.Position.y,64,16), c.Type);
			}
			if (GUI.Button(new Rect(cx, cy - 20, 20, 20), "Close")){
				Close ();
			}
		}
	}

	void Open()
	{
		Opened = true;
	}

	void Close()
	{
		var game = GameObject.Find("GameManager").GetComponent<GameManager>();
		game.Game.Scene.Containers.Where (c => c.ContainerId == ContainerId).FirstOrDefault().containedObjects = containedObjects;
		Opened = false;
	}

	void OnMouseDown()
	{
		Debug.Log ("You clicked on a container");
		if (!Opened){
			Open ();
		}
	}

	void UpdateContainer()
	{
		FindObjectOfType<GameManager>().Game.Scene.Containers.Where(c => c.ContainerId == ContainerId).FirstOrDefault().containedObjects = containedObjects;
	}
}
