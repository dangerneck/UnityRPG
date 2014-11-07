using UnityEngine;
using System.Linq;
using System.Collections;

public class CameraHideColliderManager : MonoBehaviour {

	BoxCollider collider;
	public Camera camera;
	public GameObject player;


	// Use this for initialization
	void Start () {
		collider = GetComponent<BoxCollider> ();
		collider.size = new Vector3 (5, 10, 10);
	}

	void FixedUpdate () 
	{
		collider.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
	}

	void OnTriggerEnter(Collider other)
	{
		var renderers = other.GetComponentsInChildren<Renderer>().ToList ();
		foreach(var renderer in renderers){
			renderer.enabled = false;
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
