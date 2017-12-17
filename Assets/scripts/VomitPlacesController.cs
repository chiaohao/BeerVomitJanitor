using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class VomitPlacesController : MonoBehaviour {

	public void SetVomitCameraRenderer () {
		ObiFluidRenderer playerCam = FindObjectOfType<ObiFluidRenderer> ();
		playerCam.particleRenderers = new ObiParticleRenderer[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
			playerCam.particleRenderers [i] = transform.GetChild (i).GetComponentInChildren<ObiParticleRenderer> ();
	}

	public void VomitToIndex (int i) {
		transform.GetChild (i).GetComponentInChildren<ObiEmitter> ().speed = 1;
	}

}
