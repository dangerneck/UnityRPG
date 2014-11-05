using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public float maxSpeed = 0.5f;
	public float cameraY = 5f;
	public float cameraZ = 0f;
	public float cameraX = 3f;
	public float accelerationPercent = 0.2f;
	Vector3 moveDirection;
	float moveSpeed;

	float moveLerper = 0.0f;

	public Camera camera;
	Rigidbody body;

	// Use this for initialization
	void Start () {
		body = this.rigidbody;
		camera.transform.position = new Vector3 (body.position.x + cameraX, body.position.y + cameraY, body.position.z + cameraZ);
		camera.transform.LookAt (body.position);
	}

	void FixedUpdate () {
		if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) 
		{
			if (moveLerper != 1)
			{
				if (moveLerper > 1){
					moveLerper = 1;
				}else{
					moveLerper += accelerationPercent;
				}
			}
		} else {
			if (moveLerper != 0)
			{
				moveLerper = 0;
			}
		}

		moveDirection = new Vector3(Mathf.Lerp (0, Input.GetAxis("Horizontal") * maxSpeed, moveLerper), 0, Mathf.Lerp (0, Input.GetAxis("Vertical") * maxSpeed, moveLerper));
		                           
		body.MovePosition (body.position + moveDirection);

		camera.transform.position = new Vector3 (body.position.x + cameraX, body.position.y + cameraY, body.position.z + cameraZ);
		camera.transform.LookAt (body.position);
	}
}
