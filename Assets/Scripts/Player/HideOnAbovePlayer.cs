using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class HideOnAbovePlayer : MonoBehaviour {
	
	public Transform player;
	public Camera camera;
	public BoxCollider collider;
	List<Renderer> hiddenRenderers;
	float hideAtY;
	
	// Use this for initialization
	void Start () {
		hiddenRenderers = new List<Renderer> ();
	}

	// Update is called once per frame
	void Update () {
		RaycastHit checkIntersect;
		hideAtY = -100;

		var distance = 10;
		var direction = Vector3.up;
		
		foreach(var r in hiddenRenderers)
		{
			r.enabled = true;
		}
		
		hiddenRenderers.Clear ();
		
		if (Physics.Raycast(player.transform.position, direction, out checkIntersect, distance, LayerMask.GetMask("Roof")))
		{
			if (checkIntersect.point.y > hideAtY){
				hideAtY = checkIntersect.point.y;
			}
		}
		
		
	}
}
