using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MopWashBathController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool IsMopWashAvailable(Transform player) {
		Vector3 cp = player.position;
		Vector3 bp = transform.position;
		Vector3 cf = player.forward.normalized;
		if (Vector3.Dot ((cp - bp).normalized, cf) > -0.5f)
			return false;
		float d = Vector3.Distance (cp, bp);
		if (d < 1.5f)
			return true;
		else
			return false;
	}
}
