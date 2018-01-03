using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Obi;

public class VomitEmittersController : MonoBehaviour {

	bool[] availableEmitter;
	GameObject DrunkerMouth;
	Transform CurrentEmitter;
	bool isVomitting;
	int cleaningEmitterID;

	void Start(){
		availableEmitter = new bool[transform.childCount];
		for (int i = 0; i < availableEmitter.Length; i++)
			availableEmitter [i] = true;
		isVomitting = false;
		cleaningEmitterID = -1;
	}

	void Update(){
		for (int i = 0; i < availableEmitter.Length; i++) {
			ObiEmitter e = transform.GetChild (i).GetComponent<ObiEmitter> ();
			if (!availableEmitter [i] && e.ActiveParticles == e.NumParticles)
				e.transform.position = e.GetParticlePosition (0);
		}
		if (isVomitting) {
			CurrentEmitter.position = DrunkerMouth.transform.position;
			CurrentEmitter.forward = DrunkerMouth.transform.forward;
			CurrentEmitter.GetComponent<ObiEmitter> ().speed = 0.2f;
			if (CurrentEmitter.GetComponent<ObiEmitter> ().ActiveParticles == CurrentEmitter.GetComponent<ObiEmitter> ().NumParticles)
				isVomitting = false;
		}
		if (cleaningEmitterID >= 0) {
			float s = transform.GetChild (cleaningEmitterID).GetComponent<ObiParticleRenderer> ().radiusScale;
			float ns = s - Time.deltaTime * 0.4f;
			transform.GetChild (cleaningEmitterID).GetComponent<ObiParticleRenderer> ().radiusScale = ns > 0f ? ns : 0f;
			if (ns <= 0f) {
				transform.GetChild (cleaningEmitterID).GetComponent<ObiEmitter> ().speed = 0f;
				transform.GetChild (cleaningEmitterID).GetComponent<ObiEmitter> ().KillAll ();
				transform.GetChild (cleaningEmitterID).GetComponent<ObiParticleRenderer> ().radiusScale = 1.7f;
				availableEmitter [cleaningEmitterID] = true;
				cleaningEmitterID = -1;
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
		int id = Array.IndexOf<bool> (availableEmitter, true);
		availableEmitter [id] = false;
		CurrentEmitter = transform.GetChild (id);
	}

	public int GetCleanableEmitter(Transform cleaner) {
		int id = -1;
		float minDist = float.MaxValue;

		for (int i = 0; i < availableEmitter.Length; i++) {
			if (availableEmitter[i] == true)
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
