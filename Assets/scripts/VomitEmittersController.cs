using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class VomitEmittersController : MonoBehaviour {

	int currentEmitterID;
	GameObject DrunkerMouth;
	Transform CurrentEmitter;
	bool isVomitting;

	void Start(){
		currentEmitterID = 0;
		isVomitting = false;
	}

	void Update(){
		if (isVomitting) {
			CurrentEmitter.position = DrunkerMouth.transform.position;
			CurrentEmitter.forward = DrunkerMouth.transform.forward;
			if (CurrentEmitter.GetComponent<ObiEmitter> ().ActiveParticles == CurrentEmitter.GetComponent<ObiEmitter> ().NumParticles) {
				isVomitting = false;
				currentEmitterID += 1;
			}
		}
	}

	public void SetDrunkerMouth(GameObject dm){
		DrunkerMouth = dm;
	}

	public void SetVomitCameraRenderer () {
		ObiFluidRenderer playerCam = FindObjectOfType<ObiFluidRenderer> ();
		playerCam.particleRenderers = new ObiParticleRenderer[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
			playerCam.particleRenderers [i] = transform.GetChild (i).GetComponent<ObiParticleRenderer> ();
	}

	public void VomitToIndex () {
		isVomitting = true;
		CurrentEmitter = transform.GetChild (currentEmitterID);
		transform.GetChild (currentEmitterID).GetComponent<ObiEmitter> ().speed = 0.2f;
	}

}
