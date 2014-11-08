using UnityEngine;
using System.Linq;
using System.Collections;

public class CameraHideColliderManager : MonoBehaviour {

	BoxCollider collider;
	public Camera camera;
	public GameObject player;
	PlayerControl playerControl;

	// Use this for initialization
	void Start () {
		collider = GetComponent<BoxCollider> ();
		collider.size = new Vector3 (1, 10, 1);
		playerControl = player.GetComponent<PlayerControl>();
	}

	void FixedUpdate () 
	{
		collider.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y + 6, player.transform.position.z);
	}

	void OnTriggerEnter(Collider other)
	{
	
		var bounds = other.bounds;
		if (!bounds.Contains (playerControl.playerBounds[0]) &&
		    !bounds.Contains (playerControl.playerBounds[1]) &&
		    !bounds.Contains (playerControl.playerBounds[2]) &&
		    !bounds.Contains (playerControl.playerBounds[3])){
			var renderers = other.GetComponentsInChildren<Renderer>().ToList ();
			foreach(var renderer in renderers){
				renderer.enabled = false;
			}
		}

	}

	void OnTriggerExit(Collider other)
	{
		var renderers = other.GetComponentsInChildren<Renderer>().ToList ();
		foreach(var renderer in renderers){
			renderer.enabled = true;
		}
	}
}
