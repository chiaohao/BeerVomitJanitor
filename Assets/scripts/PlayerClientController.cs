using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Obi;

public class PlayerClientController : NetworkBehaviour {

	//ServerDataController sdc;
	NetworkController nc;
	ServerDataController sdc;

	//movement
	public float speed;
	public float rotationSpeed;
	public Camera playerCamera;
	Animator animator;
	public Avatar Drunker;
	public Avatar Cleaner;
	public GameObject DrunkerObject;
	public GameObject CleanerObject;

	bool lockJump;
	bool lockVomit;
	bool lockWalk;
	bool lockClean;

	[SyncVar]
	int characterId;

	//raycast
	Ray ray;
	float rayLength = 15f;
	RaycastHit hit;
	bool isTrace;

	void Start(){
		sdc = FindObjectOfType<ServerDataController> ();
		nc = FindObjectOfType<NetworkController> ();
		animator = GetComponentInChildren<Animator> ();
		Cursor.lockState = CursorLockMode.Locked;

		if (isLocalPlayer) {
			transform.position = GameObject.FindGameObjectsWithTag ("PlayerSpawnPos")[0].transform.GetChild(0).position;
			foreach (ServerDataController.PlayerAttribute p in sdc.players) {
				if (p.NetworkId == connectionToServer.connectionId)
					characterId = p.CharacterId;
			}
			FindObjectOfType<VomitPlacesController> ().SetVomitCameraRenderer ();
		} 
		else {
			playerCamera.gameObject.SetActive (false);
			transform.position = GameObject.FindGameObjectsWithTag ("PlayerSpawnPos")[0].transform.GetChild(1).position;
		}


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
		}

		if (isLocalPlayer) {
			//Debug.Log (connectionToServer.connectionId);
			//movement
			animator.SetBool ("jump", false);
			animator.SetBool ("vomit", false);
			animator.SetBool ("clean", false);
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

			if (Input.GetAxisRaw ("Jump") != 0f && !lockJump) {
				GetComponent<Rigidbody> ().AddForce (Vector3.up * 10, ForceMode.Impulse);
				animator.SetBool ("jump", true);
				lockJump = true;
				lockVomit = true;
				StartCoroutine (unlockAction (79f / 60f));
			}

			//raycast
			ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
			if (Physics.Raycast (ray, out hit, rayLength)) {
				//Debug.Log (hit.transform.name);
				if (animator.GetBool ("Drunker")) {
					if (Input.GetAxisRaw ("Fire1") != 0f && !lockVomit) {
						Vector3 hitPos = hit.point;
						Vector3 hitNorm = hit.normal;
						/*
						//register vomit prefab
						ClientScene.RegisterPrefab (nc.spawnPrefabs [0]);
						CmdSpawnVomit (hitPos, hitNorm);
						*/
						CmdVomit (0);

						animator.SetBool ("vomit", true);
						lockVomit = true;
						lockJump = true;
						lockWalk = true;
						StartCoroutine (unlockAction (317f / 60f));
					}
				} 
				else if (animator.GetBool ("Cleaner")) {
					if (Input.GetAxisRaw ("Fire1") != 0f && !lockClean) {
						animator.SetBool ("clean", true);
						lockJump = true;
						lockWalk = true;
						StartCoroutine (unlockAction (3f * 78f / 60f));
					};
				}
			}
		}
	}

	IEnumerator unlockAction (float seconds){
		yield return new WaitForSeconds(seconds);
		lockJump = false;
		lockVomit = false;
		lockWalk = false;
		lockClean = false;
		//print ("unlock");
	}

	[Command]
	public void CmdVomit(int index){
		RpcVomit (index);
	}

	[ClientRpc]
	public void RpcVomit(int index){
		FindObjectOfType<VomitPlacesController> ().VomitToIndex (index);
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
