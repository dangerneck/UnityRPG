using UnityEngine;
using System.Linq;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public float maxSpeed = 0.5f;
	public float cameraY = 5f;
	public float cameraZ = 0f;
	public float cameraX = 3f;
	public float accelerationPercent = 0.2f;
	public float playerSize = 1.0f;
	public Vector3[] playerBounds;
	public GameObject controlFocus;
	public Camera camera;
	public PlayerStateModel State;
	public GameManager GameManager;

	Vector3 moveDirection;
	float moveSpeed;
	float moveLerper = 0.0f;
	Transform body;
	Texture Gump;
	GameObject Inventory;

	bool IsInventoryOpen = false;
	int InventoryPointer = 0;
	
	void Start () {
		body = this.transform;
		controlFocus = this.gameObject;
		camera.transform.position = new Vector3 (body.position.x + cameraX, body.position.y + cameraY, body.position.z + cameraZ);
		camera.transform.LookAt (body.position);
		playerBounds = new Vector3[4];
		Gump = GameManager.LoadedTextures.FirstOrDefault(x => x.Key == "Placeholder Container").Value;
		Inventory = GameObject.Find ("Inventory");

	}

	public void ClaimControlFocus(GameObject g = null){
		if (g == null){
			controlFocus = this.gameObject;
		}else{
			controlFocus = g;
		}
	}

	public void InventoryOpen(){
		IsInventoryOpen = true;
		controlFocus = Inventory;
	}

	public void InventoryClose(){
		IsInventoryOpen = false;
		ClaimControlFocus();
	}

	void FixedUpdate () {

		if (controlFocus == this.gameObject){
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
			moveDirection = new Vector3(Mathf.Lerp (0, Input.GetAxis("Horizontal") * maxSpeed, moveLerper), 0, Mathf.Lerp (0, Input.GetAxis("Vertical") * maxSpeed, moveLerper));
		}else{
			moveDirection = new Vector3(0,0,0);
		}

		// Collision detection for move
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

		// Inventory
		if (Input.GetKeyDown("i")){
			if (IsInventoryOpen){
				InventoryClose();
			}else{
				InventoryOpen();
			}
		}

		if (IsInventoryOpen){
			if (controlFocus == Inventory)
			{
				if (Input.GetKeyDown("left")){
					InventoryPointer--;
				}
				if (Input.GetKeyDown("right")){
					InventoryPointer++;
				}
				if (Input.GetKeyDown("up")){
					InventoryPointer -= State.Inventory.Width;
				}
				if (Input.GetKeyDown("down")){
					InventoryPointer += State.Inventory.Width;
				}
				
				if (Input.GetKeyDown("z")){
					// TODO: Use???
				}
				
				if (Input.GetKeyDown("x")){
					var o = State.Inventory.containedObjects.ElementAt(InventoryPointer);
					if (o != null){
						o.CreateInWorld(this.gameObject.transform.position);
						State.Inventory.containedObjects.Remove (o);
					}
				}
				
				if (InventoryPointer < 0){ InventoryPointer = 0; }
				if (InventoryPointer > State.Inventory.Width * State.Inventory.Height - 1){ InventoryPointer = State.Inventory.Width * State.Inventory.Height - 1; }
				
				if (Input.GetKeyDown("c")){
					InventoryClose ();
				}
				
			}
		}
	}

	void OnGUI()
	{
		if (IsInventoryOpen)
		{
			float wCenter = Screen.width/2;
			float hCenter = Screen.height/2;
			float cx = wCenter - Gump.width/2;
			float cy = hCenter - Gump.height/2;
			int centreW = State.Inventory.Width/2;
			int centreH = State.Inventory.Height/2;
			GUI.DrawTexture(new Rect(cx, cy, Gump.width, Gump.height), Gump);
			var textures = GameManager.LoadedTextures;
			
			for(int j = 0; j < State.Inventory.Width; j++){
				for (int i = 0; i < State.Inventory.Height; i++){
					float spaceX = wCenter - ((centreW - i) * 32);
					float spaceY = hCenter - ((centreH - j) * 32);
					bool active = i +(j*State.Inventory.Width) == InventoryPointer;
					GUI.DrawTexture(new Rect(spaceX, spaceY, 32, 32), active ? textures["Container Slot Active"]: textures["Container Slot"]);
					if (State.Inventory.containedObjects.Count >= (i+j*State.Inventory.Width)+1){
						if (State.Inventory.containedObjects.ElementAt (i+j*State.Inventory.Width) != null){
							GUI.DrawTexture(new Rect(spaceX, spaceY, 32, 32), textures[State.Inventory.containedObjects.ElementAt (i+j*State.Inventory.Width).Type]);
						}
					}
				}
			}
			if (GUI.Button(new Rect(cx, cy - 20, 20, 20), "x")){
				InventoryClose ();
			}
		}
	}
}
