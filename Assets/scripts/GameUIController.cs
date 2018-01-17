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
	public GameObject pukeIcon;

	public GameObject vomitGauge;
	public Image vomitGaugeFill;
	public GameObject mop;
	public Image mopFill;
	public Image dectetorFill;

	public GameObject Map1F;
	public GameObject Map2F;

	public GameObject loadingPanel;
	public GameObject winPanel;
	public Text winText;
	public Button returnBtn;

	void Start () {
		vomitGaugeFill.fillAmount = 0f;
		mopFill.fillAmount = 1f;
	}

	void Update () {
		sdc = FindObjectOfType<ServerDataController> ();
		timeText.text = Mathf.Floor (sdc.gameTime / 60f).ToString ("00") + " : " + (sdc.gameTime % 60).ToString ("00");
		FillDectetor (Mathf.Clamp01 (sdc.dirtyLevel));
	}

	void OnApplicationFocus(bool hasFocus){
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void InitPlayerUI(int i){
		if (i == 0) {
			vomitGauge.SetActive (true);
			mop.SetActive (false);
		} 
		else if (i == 1){
			vomitGauge.SetActive (false);
			mop.SetActive (true);
		}
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

	public void SetPukeIcon(bool i){
		pukeIcon.SetActive (i);
	}

	public void FillVomit(float i){
		vomitGaugeFill.fillAmount = Mathf.Clamp01 (i);
	}

	public void FillMop(float i){
		mopFill.fillAmount = 1f - Mathf.Clamp01 (i);
	}

	public void FillDectetor(float i){
		dectetorFill.transform.rotation = Quaternion.Euler(0, 0, 267f - Mathf.Clamp01 (i) * 267f);
	}

	public void CloseLoading(){
		loadingPanel.SetActive (false);
	}

	public void SetWinPanel(bool i){
		Cursor.lockState = CursorLockMode.None;
		winPanel.SetActive (true);
		winText.text = i ? "Drunker Wins!" : "Cleaner Wins!";
		returnBtn.onClick.AddListener (delegate{FindObjectOfType<GameStatusController> ().switchStatus (GameStatus.Lobby);});
		returnBtn.onClick.AddListener (delegate{FindObjectOfType<NetworkController> ().SendReturnToLobby();});
	}

	public void SetMap2F(bool i){
		Map1F.SetActive (!i);
		Map2F.SetActive (i);
	}
}
