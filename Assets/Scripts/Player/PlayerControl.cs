using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public float maxSpeed = 0.5f;
	public float cameraY = 5f;
	public float cameraZ = 0f;
	public float cameraX = 3f;
	public float accelerationPercent = 0.2f;

	public float playerSize = 1.0f;
	public Vector3[] playerBounds;

	Vector3 moveDirection;
	float moveSpeed;

	float moveLerper = 0.0f;


	public Camera camera;
	Transform body;

	// Use this for initialization
	void Start () {
		body = this.transform;
		camera.transform.position = new Vector3 (body.position.x + cameraX, body.position.y + cameraY, body.position.z + cameraZ);
		camera.transform.LookAt (body.position);
		playerBounds = new Vector3[4];
	}

	void FixedUpdate () {

		// Handle move acceleration through lerp
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

		// Collision detection for move
		moveDirection = new Vector3(Mathf.Lerp (0, Input.GetAxis("Horizontal") * maxSpeed, moveLerper), 0, Mathf.Lerp (0, Input.GetAxis("Vertical") * maxSpeed, moveLerper));
		var moveMagnitude = moveDirection.magnitude;
		var normDir = moveDirection / moveMagnitude;

		RaycastHit rayHit;
		if (!Physics.SphereCast (body.position, playerSize/2, normDir, out rayHit, moveMagnitude)) {
			body.position = body.position + moveDirection;
		}else if (!Physics.SphereCast (body.position + new Vector3(0f, 0.5f, 0f), playerSize/2, normDir, out rayHit, moveMagnitude)) {
			body.position = body.position + moveDirection + new Vector3(0f, 0.5f, 0f);
		}
	
		//Gravity collision check & move
		moveDirection = new Vector3(0f,-0.1f,0f);
		moveMagnitude = moveDirection.magnitude;
		if (!Physics.SphereCast (body.position, playerSize/2, Vector3.down, out rayHit, moveMagnitude)) {
			body.position = body.position + moveDirection;
		}

		// Update player bounds
		float halfPlayerSize = playerSize / 2;
		playerBounds[0] = new Vector3(body.position.x - halfPlayerSize, body.position.y + halfPlayerSize, body.position.z + halfPlayerSize);
		playerBounds[1] = new Vector3(body.position.x + halfPlayerSize, body.position.y + halfPlayerSize, body.position.z - halfPlayerSize);
		playerBounds[2] = new Vector3(body.position.x - halfPlayerSize, body.position.y - halfPlayerSize, body.position.z - halfPlayerSize);
		playerBounds[3] = new Vector3(body.position.x + halfPlayerSize, body.position.y - halfPlayerSize, body.position.z + halfPlayerSize);

		// Move camera to player's new position
		camera.transform.position = new Vector3 (body.position.x + cameraX, body.position.y + cameraY, body.position.z + cameraZ);
		camera.transform.LookAt (body.position);
	}
}
