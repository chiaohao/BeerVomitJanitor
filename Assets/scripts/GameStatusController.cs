using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStatus{
	MainMenu,
	HostCreating,
	ClientConnecting,
	Waiting,
	CharacterChoose,
	Playing,
	GameOver, 
	Error
}

public class GameStatusController : MonoBehaviour {

	public GameObject MainPanel;
	public GameObject ServerPanel;
	public GameObject ClientPanel;
	public GameObject WaitingPanel;
	public GameObject ErrorPanel;

	[HideInInspector]
	public GameStatus gs;

	void Start () {
		gs = GameStatus.MainMenu;
	}
	
	public void switchStatus(GameStatus s){
		switch (s) {
		case GameStatus.MainMenu:
			MainPanel.SetActive (true);
			ServerPanel.SetActive (false);
			ClientPanel.SetActive (false);
			WaitingPanel.SetActive (false);
			ErrorPanel.SetActive (false);
			break;
		case GameStatus.HostCreating:
			MainPanel.SetActive (false);
			ServerPanel.SetActive (true);
			break;
		case GameStatus.ClientConnecting:
			MainPanel.SetActive (false);
			ClientPanel.SetActive (true);
			break;
		case GameStatus.Waiting:
			WaitingPanel.SetActive (true);
			ServerPanel.SetActive (false);
			ClientPanel.SetActive (false);
			break;
		case GameStatus.CharacterChoose:
			WaitingPanel.SetActive (false);
			break;
		case GameStatus.Playing:
			break;
		case GameStatus.GameOver:
			break;
		case GameStatus.Error:
			WaitingPanel.SetActive (false);
			ErrorPanel.SetActive (true);
			break;
		default:
			break;
		}
		gs = s;
	}
}
