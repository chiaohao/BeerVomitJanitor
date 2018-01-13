using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlableCamera : MonoBehaviour {

	[SerializeField]
	private PPManager ppp;
	[SerializeField]
	private float alcoholValue = 0f;
	public float sensitivityX = 15F;
	public float runSpeed = 3,
	sprintSpeed = 20;

	// Use this for initialization
	private Transform cameraTransform;
	void Start () {
		cameraTransform = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey("w")) {
			transform.position += Vector3.forward * (Time.deltaTime * runSpeed);
		}
		if (Input.GetKey("s")) {
			transform.position += Vector3.back * (Time.deltaTime * runSpeed);
		}
		if (Input.GetKey("a")) {
			transform.position += Vector3.left * (Time.deltaTime * runSpeed);
		}
		if (Input.GetKey("d")) {
			transform.position += Vector3.right * (Time.deltaTime * runSpeed);
		}
		if (Input.GetKey("q")) {
			transform.position += Vector3.down * (Time.deltaTime * runSpeed);
		}
		if (Input.GetKey("e")) {
			transform.position += Vector3.up * (Time.deltaTime * runSpeed);
		}
		if (Input.GetKeyDown ("p")) {
			ppp.toggleHeavyEffect ();
		}
		if (Input.GetKeyDown ("9")) {
			alcoholValue = Mathf.Clamp01 (alcoholValue - 0.1f);
			ppp.SetAlcoholValue (alcoholValue);
		}
		if (Input.GetKeyDown ("0")) {
			alcoholValue = Mathf.Clamp01 (alcoholValue + 0.1f);
			ppp.SetAlcoholValue (alcoholValue);
		}
	}
}
