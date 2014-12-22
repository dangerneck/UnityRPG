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
	float halfPlayerSize;
	bool IsInventoryOpen = false;
	int InventoryPointer = 0;
	float wCenter;
	float hCenter;
	float cx;
	float cy;
	float thirdX;
	float thirdY;
	float halfGumpWidth;
	int centreW;
	int centreH;
	
	void Start () {
		body = this.transform;
		controlFocus = this.gameObject;
		camera.transform.position = new Vector3 (body.position.x + cameraX, body.position.y + cameraY, body.position.z + cameraZ);
		camera.transform.LookAt (body.position);
		playerBounds = new Vector3[4];
		Gump = GameManager.LoadedTextures.FirstOrDefault(x => x.Key == "Placeholder Container").Value;
		Inventory = GameObject.Find ("Inventory");
		halfPlayerSize = playerSize / 2;
		wCenter = Screen.width/2;
		hCenter = Screen.height/2;
		thirdX = Screen.width / 3;
		thirdY = Screen.height / 3;
		halfGumpWidth = Gump.width / 2;
		cx = thirdX - Gump.width/2;
		cy = hCenter - Gump.height/2;
		centreW = State.Inventory.Width/2;
		centreH = State.Inventory.Height/2;
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

	public void Teleport(TeleportModel m)
	{
		if (m.To == "local"){
			transform.position = m.ToPosition;
		}else{
			GameManager.SendMessage("ChangeScene", new ChangeSceneModel{
				SceneId = m.To,
				OnComplete = () => {
					if (m.ToPosition != null){
						GameObject.Find ("Player").transform.position = m.ToPosition;
					}
				}
			});
		}
	}

	void FixedUpdate () {

		if (controlFocus == this.gameObject){
			HandleMove();
			HandleGeneralInput();
		}else if (controlFocus == Inventory){
			HandleInventoryInput();
		}

		// Update player bounds
		playerBounds[0] = new Vector3(body.position.x - halfPlayerSize, body.position.y + halfPlayerSize, body.position.z + halfPlayerSize);
		playerBounds[1] = new Vector3(body.position.x + halfPlayerSize, body.position.y + halfPlayerSize, body.position.z - halfPlayerSize);
		playerBounds[2] = new Vector3(body.position.x - halfPlayerSize, body.position.y - halfPlayerSize, body.position.z - halfPlayerSize);
		playerBounds[3] = new Vector3(body.position.x + halfPlayerSize, body.position.y - halfPlayerSize, body.position.z + halfPlayerSize);

		// Move camera to player's new position
		camera.transform.position = new Vector3 (body.position.x + cameraX, body.position.y + cameraY, body.position.z + cameraZ);
		camera.transform.LookAt (body.position);
	}

	void OnGUI()
	{
		if (IsInventoryOpen)
		{
			GUI.DrawTexture(new Rect(cx, cy, Gump.width, Gump.height), Gump);
			Rect descriptionBox = new Rect(thirdX + halfGumpWidth, cy, halfGumpWidth, Gump.height);
			if (State.Inventory.containedObjects.Count > InventoryPointer){
				if (State.Inventory.containedObjects.ElementAt (InventoryPointer) != null){
					GUI.Box (descriptionBox,State.Inventory.containedObjects.ElementAt(InventoryPointer).Type);
					// TODO: add a dscriptions lookup for item by type to display more shit.
				}
			}

			    
			var textures = GameManager.LoadedTextures;			
			for(int j = 0; j < State.Inventory.Width; j++){
				for (int i = 0; i < State.Inventory.Height; i++){
					float spaceX = thirdX - ((centreW - i) * 32);
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

	void HandleMove()
	{
		// Handle move acceleration through lerp
		if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) 
		{
			if (moveLerper != 1)
			{
				if (moveLerper < 1){
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
	}

	void HandleGeneralInput()
	{
		if (Input.GetKeyDown("i")){
			if (IsInventoryOpen){
				InventoryClose();
			}else{
				InventoryOpen();
			}
		}
		
		if (Input.GetKeyDown ("x")){
			RaycastHit grabRayHit;
			if (Physics.SphereCast(body.position, playerSize/6, new Vector3(0,-1,0), out grabRayHit, 1.0f)){
				var item = grabRayHit.collider.GetComponentInParent<Item>();
				if (item != null){
					if (!State.Inventory.IsFull){
						State.Inventory.containedObjects.Add (item.State);
						item.SendMessageUpwards("Destroy");
					}
				}
			}
		}
	}

	void HandleInventoryInput(){
		if (IsInventoryOpen){
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
					o.CreateInWorld(new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 0.5f, this.gameObject.transform.position.z));
					State.Inventory.containedObjects.Remove (o);
					GameManager.Game.Scene.Objects.Add(o);
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
