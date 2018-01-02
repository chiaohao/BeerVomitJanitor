using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsController : MonoBehaviour {

	bool[] doorStatuses; // 0 for close, 1 for open

	// Use this for initialization
	void Start () {
		doorStatuses = new bool[transform.childCount];
		for (int i = 0; i < doorStatuses.Length; i++)
			doorStatuses [i] = false; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetAvailableDoor(Transform player) {
		int id = -1;
		float minDist = float.MaxValue;

		for (int i = 0; i < transform.childCount; i++) {
			Vector3 cp = player.position;
			Vector3 dp = transform.GetChild (i).position;
			Vector3 cf = player.forward.normalized;
			if (Vector3.Dot ((cp - dp).normalized, cf) > -0.5f)
				continue;
			float d = Vector3.Distance (cp, dp);
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

	public void SwitchDoor(int id){
		doorStatuses [id] = doorStatuses[id] ? false : true;
		if (doorStatuses [id])
			transform.GetChild (id).GetChild (0).GetComponent<Animation> ().Play ("DoorOpen");
		else
			transform.GetChild (id).GetChild (0).GetComponent<Animation> ().Play ("DoorClose");
	}
}
