using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Obi;

public enum handItems{
	none,
	broom,
	bottle,
}

public class PlayerClientController : NetworkBehaviour {

	//ServerDataController sdc;
	NetworkController nc;
	ServerDataController sdc;
	GameUIController guic;

	//movement
	public float speed;
	public float rotationSpeed;
	public Camera playerCamera;
	Animator animator;
	public Avatar Drunker;
	public Avatar Cleaner;
	public GameObject DrunkerObject;
	public GameObject CleanerObject;
	public GameObject DrunkerMouth;

	bool isAnimatedSpecial;
	bool lockJump;
	bool lockVomit;
	bool lockWalk;
	bool lockClean;
	bool waitingAction;

	//syncvar
	[SyncVar]
	int characterId;
	[SyncVar]
	int drunkLevel;
	[SyncVar]
	handItems handItem;
	[SyncVar]
	int broomDirtLevel;

	//raycast
	Ray ray;
	float rayLength = 15f;
	RaycastHit hit;
	bool isTrace;

	void Start(){
		sdc = FindObjectOfType<ServerDataController> ();
		nc = FindObjectOfType<NetworkController> ();
		guic = FindObjectOfType<GameUIController> ();
		animator = GetComponentInChildren<Animator> ();
		Cursor.lockState = CursorLockMode.Locked;

		if (isLocalPlayer) {
			transform.position = GameObject.FindGameObjectsWithTag ("PlayerSpawnPos")[0].transform.GetChild(0).position;
			foreach (ServerDataController.PlayerAttribute p in sdc.players) {
				if (p.NetworkId == connectionToServer.connectionId)
					characterId = p.CharacterId;
			}
			FindObjectOfType<VomitEmittersController> ().SetVomitCameraRenderer ();
		} 
		else {
			playerCamera.gameObject.SetActive (false);
			transform.position = GameObject.FindGameObjectsWithTag ("PlayerSpawnPos")[0].transform.GetChild(1).position;
		}

		drunkLevel = 1;
		handItem = handItems.none;
		broomDirtLevel = 0;

		isAnimatedSpecial = true;
		lockJump = false;
		lockVomit = false;
		lockWalk = false;
		lockClean = false;

		isTrace = true;
	}


	void Update(){
		if (characterId == 1) {
			animator.SetBool ("Drunker", false);
			animator.SetBool ("Cleaner", true);
			DrunkerObject.SetActive (false);
			CleanerObject.SetActive (true);
			animator.avatar = Cleaner;
		} 
		else if (characterId == 0) {
			animator.SetBool ("Drunker", true);
			animator.SetBool ("Cleaner", false);
			DrunkerObject.SetActive (true);
			CleanerObject.SetActive (false);
			animator.avatar = Drunker;
			FindObjectOfType<VomitEmittersController> ().SetDrunkerMouth (DrunkerMouth);
		}

		if (isLocalPlayer) {
			//Debug.Log (animator.GetCurrentAnimatorStateInfo (0).IsName("Drunker Jump"));
			//Debug.Log (connectionToServer.connectionId);
			//movement
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Drunker Jump") || animator.GetCurrentAnimatorStateInfo (0).IsName ("Cleaner Jump")) {
				isAnimatedSpecial = true;
				animator.SetBool ("jump", false);
			} 
			else if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Drunker Vomit")) {
				isAnimatedSpecial = true;
				animator.SetBool ("vomit", false);
			} 
			else if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Cleaner Clean")) {
				isAnimatedSpecial = true;
				animator.SetBool ("clean", false);
			} 
			else {
				if (isAnimatedSpecial)
					unlockAction ();
			}
			
			if (!lockWalk) {
				Vector3 move = Vector3.Normalize (Input.GetAxis ("Horizontal") * transform.right + Input.GetAxis ("Vertical") * transform.forward) * speed * Time.deltaTime;
				transform.Translate (move, Space.World);
				animator.SetFloat ("speed", (Vector3.Dot(transform.forward, move) >= 0 ? 1 : -1) * move.magnitude / Time.deltaTime);

				Vector3 rx = Vector3.up * Input.GetAxis ("Mouse X") * rotationSpeed;
				transform.Rotate (rx);
				float ry = Input.GetAxis ("Mouse Y");
				if (ry != 0) {
					if (Mathf.Round (playerCamera.transform.rotation.eulerAngles.x) <= 30 && playerCamera.transform.rotation.eulerAngles.x + Vector3.left.x * ry * rotationSpeed > 30)
						playerCamera.transform.rotation = Quaternion.Euler (new Vector3 (30, playerCamera.transform.rotation.eulerAngles.y, playerCamera.transform.rotation.eulerAngles.z));
					else if (Mathf.Round (playerCamera.transform.rotation.eulerAngles.x) >= 0 && playerCamera.transform.rotation.eulerAngles.x + Vector3.left.x * ry * rotationSpeed < 0)
						playerCamera.transform.rotation = Quaternion.Euler (new Vector3 (0, playerCamera.transform.rotation.eulerAngles.y, playerCamera.transform.rotation.eulerAngles.z));
					else
						playerCamera.transform.Rotate (Vector3.left * ry * rotationSpeed);
				}
			}

			if (!waitingAction) {
				if (Input.GetAxisRaw ("Jump") != 0f && !lockJump) {
					GetComponent<Rigidbody> ().AddForce (Vector3.up * 1, ForceMode.Impulse);
					animator.SetBool ("jump", true);
					StartCoroutine(waiting ());
					lockJump = true;
					lockVomit = true;
				}
					
				int nearestDoorID = FindObjectOfType<DoorsController> ().GetAvailableDoor (transform);
				guic.SetDoorIcon (nearestDoorID == -1 ? false : true);
				if (nearestDoorID != -1) {
					if (Input.GetKeyDown (KeyCode.E)) {
						CmdOpenDoor (nearestDoorID);
						StartCoroutine(waiting ());
					}
				}

				//special actions
				if (animator.GetBool ("Drunker")) {
					if (Input.GetButtonDown ("Fire1") && !lockVomit && drunkLevel >= 2) {
						CmdVomit ();
						drunkLevel -= 2;
						animator.SetBool ("vomit", true);
						isAnimatedSpecial = false;
						lockVomit = true;
						lockJump = true;
						lockWalk = true;
						StartCoroutine(waiting ());
					}
					int nearestBottleID = FindObjectOfType<BeerBottlesController> ().GetAvailableBottle (transform);
					guic.SetDrinkIcon (nearestBottleID == -1 ? false : true);
					if (nearestBottleID != -1) {
						if (Input.GetButtonDown ("Fire2")) {
							FindObjectOfType<BeerBottlesController> ().DrinkBottle (nearestBottleID);
							CmdDrink (nearestBottleID);
							drunkLevel += 1;
						}
					}
				} else if (animator.GetBool ("Cleaner")) {
					int nearestEmitterID = FindObjectOfType<VomitEmittersController> ().GetCleanableEmitter (transform);
					guic.SetBroomIcon (nearestEmitterID == -1 ? false : true);
					if (nearestEmitterID != -1) {
						if (Input.GetButtonDown ("Fire1") && !lockClean) {
							CmdClean (nearestEmitterID);
							animator.SetBool ("clean", true);
							isAnimatedSpecial = false;
							lockJump = true;
							lockWalk = true;
							StartCoroutine(waiting ());
						}
					}
				}
			}
		}
	}

	void unlockAction (){
		lockJump = false;
		lockVomit = false;
		lockWalk = false;
		lockClean = false;
		isAnimatedSpecial = true;
	}

	IEnumerator waiting(){
		yield return new WaitForSeconds (0.5f);
		waitingAction = false;
	}

	[Command]
	public void CmdVomit(){
		RpcVomit ();
	}

	[ClientRpc]
	public void RpcVomit(){
		FindObjectOfType<VomitEmittersController> ().VomitToIndex ();
	}

	[Command]
	public void CmdClean(int id){
		RpcClean (id);
	}

	[ClientRpc]
	public void RpcClean(int id){
		FindObjectOfType<VomitEmittersController> ().CleanEmitter (id);
	}

	[Command]
	public void CmdDrink(int id){
		RpcDrink (id);
	}

	[ClientRpc]
	public void RpcDrink(int id){
		FindObjectOfType<BeerBottlesController> ().DrinkBottle (id);
	}

	[Command]
	public void CmdOpenDoor(int id){
		RpcOpenDoor (id);
	}

	[ClientRpc]
	public void RpcOpenDoor(int id){
		FindObjectOfType<DoorsController> ().SwitchDoor (id);
	}
	/*
	[Command]
	public void CmdSpawnVomit(Vector3 position, Vector3 normal){
		SpawnVomit (position, normal);
	}

	[Server]
	void SpawnVomit(Vector3 position, Vector3 normal){
		Transform vomit = Instantiate (nc.spawnPrefabs [0], position, new Quaternion (0, 0, 0, 0)).transform;
		vomit.up = normal.normalized;
		NetworkServer.Spawn (vomit.gameObject);
	}
	*/
}
