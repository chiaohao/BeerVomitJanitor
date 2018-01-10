using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingCtrl : MonoBehaviour {
	[SerializeField]
	private Renderer render;
	[SerializeField]
	private float interval = 1.0f;
	[SerializeField]
	private float activeTime = 0.2f;
	// Use this for initialization
	void Start () {
		render = GetComponent<Renderer> ();
		InvokeRepeating("BlinkEmisson", 0.5f, interval);
		InvokeRepeating ("BlinkDark", 0.5f + activeTime, interval);
	}

	void BlinkEmisson(){
		render.material.SetColor ("_EmissionColor", new Color (0.25f, 0.25f, 0.25f));
	}
	void BlinkDark(){
		render.material.SetColor("_EmissionColor", Color.black);
	}

}
