using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class VomitEmittersController : MonoBehaviour {

	int currentEmitterID;
	GameObject DrunkerMouth;
	Transform CurrentEmitter;
	bool isVomitting;
	int cleaningEmitterID;

	void Start(){
		currentEmitterID = 0;
		isVomitting = false;
		cleaningEmitterID = -1;
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
		if (cleaningEmitterID >= 0) {
			float s = transform.GetChild (cleaningEmitterID).GetComponent<ObiParticleRenderer> ().radiusScale;
			float ns = s - Time.deltaTime * 0.2f;
			transform.GetChild (cleaningEmitterID).GetComponent<ObiParticleRenderer> ().radiusScale = ns > 0f ? ns : 0f;
			if (ns <= 0f)
				cleaningEmitterID = -1;
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

	public int GetCleanableEmitter(Transform cleaner) {
		int id = -1;
		float minDist = float.MaxValue;

		for (int i = 0; i < currentEmitterID; i++) {
			if (transform.GetChild (i).GetComponent<ObiParticleRenderer> ().radiusScale == 0 || transform.GetChild (i).GetComponent<ObiEmitter> ().ActiveParticles == 0)
				continue;
			Vector3 cp = cleaner.position;
			Vector3 ep = transform.GetChild (i).position;
			Vector3 cf = cleaner.forward.normalized;
			if (Vector3.Dot ((cp - ep).normalized, cf) > -0.5f)
				continue;
			float d = Vector3.Distance (cp, ep);
			if (d < minDist) {
				minDist = d;
				id = i;
			}
		}
		if (minDist > 1.5f)
			return -1;
		else
			return id;
	}

	public void CleanEmitter(int id){
		cleaningEmitterID = id;
	}
}
