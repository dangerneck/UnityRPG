using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Container : MonoBehaviour
{

	public List<ContainedObjectModel> containedObjects;
	public string ContainerId;

	// Use this for initialization
	void Start ()
	{
		containedObjects = FindObjectOfType<GameManager>().Game.Scene.Containers.Where(c => c.ContainerId == ContainerId).FirstOrDefault().containedObjects;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void UpdateContainer()
	{
		FindObjectOfType<GameManager>().Game.Scene.Containers.Where(c => c.ContainerId == ContainerId).FirstOrDefault().containedObjects = containedObjects;
	}
}
