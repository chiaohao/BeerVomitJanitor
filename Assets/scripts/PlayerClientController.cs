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
	BeerBottlesController bbc;

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
	public GameObject Mop;

	public AudioClip CleanMopSound;
	public AudioClip DoorOpenSound;
	public AudioClip DrinkSound;
	public AudioClip MopSound;
	public AudioClip VomitMixSound;


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
	float drunkLevel;
	[SyncVar]
	handItems handItem;
	[SyncVar]
	float mopDirtLevel;

	void Start(){
		sdc = FindObjectOfType<ServerDataController> ();
		nc = FindObjectOfType<NetworkController> ();
		guic = FindObjectOfType<GameUIController> ();
		bbc = FindObjectOfType<BeerBottlesController> ();
		animator = GetComponentInChildren<Animator> ();
		Cursor.lockState = CursorLockMode.Locked;

		if (isLocalPlayer) {
			foreach (ServerDataController.PlayerAttribute p in sdc.players) {
				if (p.NetworkId == connectionToServer.connectionId)
					characterId = p.CharacterId;
				FindObjectOfType<GameUIController> ().InitPlayerUI (characterId);
			}
			FindObjectOfType<VomitEmittersController> ().SetVomitCameraRenderer ();
		} 
		else {
			SetLayer (gameObject, 12);
			playerCamera.gameObject.SetActive (false);
		}

		drunkLevel = 1f;
		FindObjectOfType<GameUIController> ().FillVomit (drunkLevel);
		handItem = handItems.none;
		mopDirtLevel = 0f;

		isAnimatedSpecial = true;
		lockJump = false;
		lockVomit = false;
		lockWalk = false;
		lockClean = false;

		StartCoroutine (WaitLoading ());
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

		Mop.SetActive (animator.GetCurrentAnimatorStateInfo (0).IsName ("Cleaner Clean") ||
			animator.GetCurrentAnimatorStateInfo (0).IsName ("Cleaner Clean 0") || 
			animator.GetCurrentAnimatorStateInfo (0).IsName ("Cleaner Clean 1")
			? true : false);

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
			else if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Cleaner Clean") || 
				animator.GetCurrentAnimatorStateInfo (0).IsName ("Cleaner Clean 0") || 
				animator.GetCurrentAnimatorStateInfo (0).IsName ("Cleaner Clean 1")) {
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
					if (drunkLevel >= 0.2f) {
						guic.SetPukeIcon (true);
						if (Input.GetButtonDown ("Fire1") && !lockVomit) {
							CmdVomit ();
							drunkLevel -= 0.2f;
							drunkLevel = Mathf.Clamp01 (drunkLevel);
							guic.FillVomit (drunkLevel);
							animator.SetBool ("vomit", true);
							isAnimatedSpecial = false;
							lockVomit = true;
							lockJump = true;
							lockWalk = true;
							StartCoroutine (waiting ());
						}
					} 
					else {
						guic.SetPukeIcon (false);
					}
					int nearestBottleID = bbc.GetAvailableBottle (transform);
					guic.SetDrinkIcon (nearestBottleID == -1 ? false : true);
					if (nearestBottleID != -1) {
						if (Input.GetButtonDown ("Fire2")) {
							bbc.DrinkBottle (nearestBottleID);
							CmdDrink (nearestBottleID);
							drunkLevel += 0.1f;
							drunkLevel = Mathf.Clamp01 (drunkLevel);
							guic.FillVomit (drunkLevel);
						}
					}
				} else if (animator.GetBool ("Cleaner")) {
					guic.SetPukeIcon (false);
					int nearestEmitterID = FindObjectOfType<VomitEmittersController> ().GetCleanableEmitter (transform);
					bool isMopwashable = false;
					MopWashBathController[] mwbcs = FindObjectsOfType<MopWashBathController> ();
					foreach (MopWashBathController mwbc in mwbcs)
						isMopwashable = isMopwashable || mwbc.IsMopWashAvailable (transform);
					guic.SetBroomIcon (isMopwashable || nearestEmitterID != -1 ? true : false);

					if (nearestEmitterID != -1) {
						if (Input.GetButtonDown ("Fire1") && mopDirtLevel < 0.8f && !lockClean) {
							CmdClean (nearestEmitterID);
							mopDirtLevel += 0.35f;
							mopDirtLevel = Mathf.Clamp01 (mopDirtLevel);
							guic.FillMop (mopDirtLevel);
							animator.SetBool ("clean", true);
							isAnimatedSpecial = false;
							lockJump = true;
							lockWalk = true;
							StartCoroutine(waiting ());
						}
					}
						
					if (isMopwashable) {
						if (Input.GetButtonDown ("Fire1") && !lockClean) {
							CmdWashMop ();
							mopDirtLevel -= 0.5f;
							mopDirtLevel = Mathf.Clamp01 (mopDirtLevel);
							guic.FillMop (mopDirtLevel);
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

	void SetLayer(GameObject o, int layer){
		if (o.layer == 11)
			o.layer = layer;
		foreach (Transform t in o.transform)
			SetLayer (t.gameObject, layer);
	}

	IEnumerator WaitLoading(){
		yield return new WaitForSeconds (5);
		guic.loadingPanel.SetActive (false);
		if (characterId == 0)
			transform.position = GameObject.FindGameObjectsWithTag ("PlayerSpawnPos") [0].transform.GetChild (0).position;
		else
			transform.position = GameObject.FindGameObjectsWithTag ("PlayerSpawnPos") [0].transform.GetChild (1).position;
		//init bottles
		if (isServer && isLocalPlayer) {
			int disableBottleCount = bbc.availableBottle.Length - bbc.quantity;
			List<int> ids = new List<int> ();
			while (disableBottleCount != 0) {
				int id = Random.Range (0, bbc.availableBottle.Length);
				if (!ids.Contains (id)) {
					ids.Add (id);
					disableBottleCount -= 1;
				}
			}
			int[] _ids = new int[ids.Count];
			for (int i = 0; i < ids.Count; i++)
				_ids [i] = ids [i];
			CmdInitBottle (_ids);
		}
	}

	IEnumerator waiting(){
		yield return new WaitForSeconds (0.5f);
		waitingAction = false;
	}

	[Command]
	public void CmdVomit(){
		FindObjectOfType<ServerDataController> ().dirtyLevel += 0.1f;
		RpcVomit ();
	}

	[ClientRpc]
	public void RpcVomit(){
		FindObjectOfType<VomitEmittersController> ().VomitToIndex ();
		transform.GetComponent<AudioSource> ().PlayOneShot (VomitMixSound);
	}

	[Command]
	public void CmdClean(int id){
		FindObjectOfType<ServerDataController> ().dirtyLevel -= 0.1f;
		RpcClean (id);
	}

	[ClientRpc]
	public void RpcClean(int id){
		FindObjectOfType<VomitEmittersController> ().CleanEmitter (id);
		transform.GetComponent<AudioSource> ().PlayOneShot (MopSound);
	}

	[Command]
	public void CmdWashMop(){
		RpcWashMop ();
	}

	[ClientRpc]
	public void RpcWashMop(){
		transform.GetComponent<AudioSource> ().PlayOneShot (CleanMopSound);
		StartCoroutine (WashClip ());
	}

	[Command]
	public void CmdDrink(int id){
		RpcDrink (id);
	}

	[ClientRpc]
	public void RpcDrink(int id){
		bbc.DrinkBottle (id);
		transform.GetComponent<AudioSource> ().PlayOneShot (DrinkSound);
	}

	[Command]
	public void CmdOpenDoor(int id){
		RpcOpenDoor (id);
	}

	[ClientRpc]
	public void RpcOpenDoor(int id){
		FindObjectOfType<DoorsController> ().SwitchDoor (id);
		transform.GetComponent<AudioSource> ().PlayOneShot (DoorOpenSound);
	}

	IEnumerator WashClip(){
		yield return new WaitForSeconds (2f);
		transform.GetComponent<AudioSource> ().Stop ();
	}

	[Command]
	public void CmdInitBottle(int[] ids){
		RpcInitBottle (ids);
	}

	[ClientRpc]
	public void RpcInitBottle(int[] ids){
		bbc.InitBottle (ids);
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
