using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerClientController : NetworkBehaviour {

	public float speed;
	public Camera playerCamera;

	void Start(){
		if (isLocalPlayer)
			playerCamera.gameObject.SetActive (true);
	}


	void Update(){
		if (isLocalPlayer) {
			Vector3 move = Vector3.zero;
			transform.Translate(Vector3.Normalize (Input.GetAxis ("Horizontal") * transform.right + Input.GetAxis ("Vertical") * transform.forward) * speed * Time.deltaTime);
		}
	}
}
