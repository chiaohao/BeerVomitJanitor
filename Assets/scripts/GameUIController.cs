using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour {

	ServerDataController sdc;
	public Text timeText;
	public GameObject broomIcon;
	public GameObject drinkIcon;
	public GameObject doorIcon;

	void Start () {
		
	}

	void Update () {
		sdc = FindObjectOfType<ServerDataController> ();
		timeText.text = Mathf.Floor (sdc.gameTime / 60f).ToString ("00") + " : " + (sdc.gameTime % 60).ToString ("00");
	}

	public void SetBroomIcon(bool i){
		broomIcon.SetActive (i);
	}

	public void SetDrinkIcon(bool i){
		drinkIcon.SetActive (i);
	}
	public void SetDoorIcon(bool i){
		doorIcon.SetActive (i);
	}
}
