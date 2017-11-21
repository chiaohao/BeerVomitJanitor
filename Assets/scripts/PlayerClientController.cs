using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerClientController : NetworkBehaviour {

	//ServerDataController sdc;
	NetworkController nc;

	//movement
	public float speed;
	public float rotationSpeed;
	public Camera playerCamera;
	Animator animator;

	bool lockJump;
	bool lockVomit;

	//raycast
	Ray ray;
	float rayLength = 10f;
	RaycastHit hit;
	bool isTrace;

	void Start(){
		//sdc = FindObjectOfType<ServerDataController> ();
		nc = FindObjectOfType<NetworkController> ();
		Cursor.lockState = CursorLockMode.Locked;
		if (isLocalPlayer) {
			playerCamera.gameObject.SetActive (true);
			transform.position = new Vector3 (-26.54303f, 0f, 14.64f);
		} 
		else {
			transform.position = new Vector3 (-26.54303f, 0f, 15.64f);
		}
		animator = GetComponentInChildren<Animator> ();
		lockJump = false;
		lockVomit = false;

		isTrace = true;
	}


	void Update(){
		if (isLocalPlayer) {
			//movement
			animator.SetBool ("jump", false);
			animator.SetBool ("vomit", false);

			Vector3 move = Vector3.Normalize (Input.GetAxis ("Horizontal") * transform.right + Input.GetAxis ("Vertical") * transform.forward) * speed * Time.deltaTime;
			transform.Translate(move, Space.World);
			animator.SetFloat ("speed", move.magnitude / Time.deltaTime);

			Vector3 rx = Vector3.up * Input.GetAxis("Mouse X") * rotationSpeed;
			transform.Rotate (rx);
			float ry = Input.GetAxis("Mouse Y");
			if (ry != 0) {
				if (Mathf.Round(playerCamera.transform.rotation.eulerAngles.x) <= 90 && playerCamera.transform.rotation.eulerAngles.x + Vector3.left.x * ry * rotationSpeed > 90)
					playerCamera.transform.rotation = Quaternion.Euler(new Vector3(90, playerCamera.transform.rotation.eulerAngles.y, playerCamera.transform.rotation.eulerAngles.z));
				else if (Mathf.Round(playerCamera.transform.rotation.eulerAngles.x) >= 270 && playerCamera.transform.rotation.eulerAngles.x + Vector3.left.x * ry * rotationSpeed < 270)
					playerCamera.transform.rotation = Quaternion.Euler(new Vector3(270, playerCamera.transform.rotation.eulerAngles.y, playerCamera.transform.rotation.eulerAngles.z));
				else
					playerCamera.transform.Rotate(Vector3.left * ry * rotationSpeed);
			}

			if (Input.GetAxisRaw ("Jump") != 0f && !lockJump) {
				GetComponent<Rigidbody> ().AddForce (Vector3.up * 70, ForceMode.Force);
				animator.SetBool ("jump", true);
				lockJump = true;
				lockVomit = true;
				StartCoroutine (unlockAction (2f));
			}

			//raycast
			ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
			if (Physics.Raycast (ray, out hit, rayLength)) {
				//Debug.Log (hit.transform.name);
				if (Input.GetAxisRaw ("Fire1") != 0f && !lockVomit){
					Vector3 hitPos = hit.point;
					Vector3 hitNorm = hit.normal;
					//register vomit prefab
					ClientScene.RegisterPrefab (nc.spawnPrefabs [0]);
					CmdSpawnVomit (hitPos, hitNorm);
					animator.SetBool ("vomit", true);
					lockVomit = true;
					lockJump = true;
					StartCoroutine (unlockAction (5f));
				}
			}
		}
	}

	IEnumerator unlockAction (float seconds){
		yield return new WaitForSeconds(seconds);
		lockJump = false;
		lockVomit = false;
		//print ("unlock");
	}

	[Command]
	public void CmdSpawnVomit(Vector3 position, Vector3 normal){
		SpawnVomit (position, normal);
	}

	[Server]
	void SpawnVomit(Vector3 position, Vector3 normal){
		Transform vomit = Instantiate (nc.spawnPrefabs [0], position, new Quaternion (0, 0, 0, 0)).transform;
		vomit.forward = normal.normalized;
		NetworkServer.Spawn (vomit.gameObject);
	}
}
