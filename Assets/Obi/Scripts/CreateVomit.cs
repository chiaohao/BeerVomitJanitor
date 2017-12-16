using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class CreateVomit : MonoBehaviour {

	public GameObject Camera;
	public GameObject Vomit;
	private GameObject V;
	// Use this for initialization
	void Start () {
		V = Instantiate(Vomit, new Vector3(0f, 4f, 0f), Quaternion.Euler(-45, 90, 0));
		Debug.Log (Camera.GetComponents<ObiFluidRenderer> ()[0].particleRenderers[1] = V.transform.GetChild (0).GetComponent<ObiParticleRenderer> ());
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F)){
			V.transform.GetChild (0).GetComponent<ObiEmitter> ().speed = 4;
		}
	}
}
