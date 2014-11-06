using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class HideOnAbovePlayer : MonoBehaviour {
	
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

		var distance = 5;
		var direction = Vector3.up;
		
		foreach(var r in hiddenRenderers)
		{
			r.enabled = true;
		}
		
		hiddenRenderers.Clear ();
		
		if (Physics.Raycast(player.transform.position, direction, out checkIntersect, distance, LayerMask.GetMask("Roof")))
		{
			hiddenRenderers.Add(checkIntersect.transform.renderer);
			checkIntersect.transform.renderer.enabled = false;
		}
		
		
	}
}
