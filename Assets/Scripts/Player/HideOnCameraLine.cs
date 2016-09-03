using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class HideOnCameraLine : MonoBehaviour {

	public Transform player;
	public Camera camera;
	List<Renderer> hiddenRenderers;

	// Use this for initialization
	void Start () {
		hiddenRenderers = new List<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit checkIntersect;
		var toPlayer = player.position - camera.transform.position;
		var distance = toPlayer.magnitude;
		var direction = toPlayer / distance;

		foreach(var r in hiddenRenderers)
		{
			r.enabled = true;
		}

		hiddenRenderers.Clear ();

		if (Physics.Raycast(camera.transform.position, direction, out checkIntersect, distance))
		{
			hiddenRenderers.Add(checkIntersect.transform.GetComponent<Renderer>());
			checkIntersect.transform.GetComponent<Renderer>().enabled = false;
		}


	}
}
