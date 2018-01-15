using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerBottlesController : MonoBehaviour {

	public int quantity = 30;
	public bool[] availableBottle;

	// Use this for initialization
	void Start () {
		availableBottle = new bool[transform.childCount];
		for (int i = 0; i < availableBottle.Length; i++)
			availableBottle [i] = true;
		/*
		int disableBottle = availableBottle.Length - quantity;
		if(disableBottle > 0) for (int i = 0; i < disableBottle; i++) {
				int id = Random.Range (0, availableBottle.Length);
				if (availableBottle [id]) {
					DrinkBottle (id);
				}
		}
		*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InitBottle(int[] ids){
		for (int i = 0; i < ids.Length; i++)
			DrinkBottle (ids [i]);
	}

	public int GetAvailableBottle(Transform drunker) {
		int id = -1;
		float minDist = float.MaxValue;

		for (int i = 0; i < availableBottle.Length; i++) {
			if (availableBottle[i] == false)
				continue;
			Vector3 cp = drunker.position;
			Vector3 bp = transform.GetChild (i).position;
			Vector3 cf = drunker.forward.normalized;
			if (Vector3.Dot ((cp - bp).normalized, cf) > -0.5f)
				continue;
			float d = Vector3.Distance (cp, bp);
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

	public void DrinkBottle(int id){
		transform.GetChild (id).GetComponent<MeshRenderer> ().enabled = false;
		availableBottle [id] = false;
	}
}
