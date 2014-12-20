using UnityEngine;
using System.Collections;

public class PlayerTeleport : MonoBehaviour {

	public float Width;
	public float Height;
	public float Depth;
	public string To;
	public Vector3 ToPosition;

	GameObject player;
	float hWidth;
	float hHeight;
	float hDepth;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		hWidth = Width / 2;
		hHeight = Height / 2;
		hDepth = Depth / 2;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float xDistance = Mathf.Abs (transform.position.x - player.transform.position.x);
		float yDistance = Mathf.Abs (transform.position.y - player.transform.position.y);
		float zDistance = Mathf.Abs (transform.position.z - player.transform.position.z);

		if (xDistance <  hWidth && yDistance < hHeight && zDistance < hDepth){
			player.SendMessage("Teleport", new TeleportModel{
				To = To,
				ToPosition = ToPosition
			});
		}
	}
}
