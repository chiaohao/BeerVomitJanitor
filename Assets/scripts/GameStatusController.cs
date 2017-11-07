using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStatus{
	MainMenu,
	HostCreating,
	ClientConnecting,
	Lobby,
	CharacterChoose,
	Playing,
	GameOver, 
	Error
}

public class GameStatusController : MonoBehaviour {

	public GameObject MainPanel;
	public GameObject ServerPanel;
	public GameObject ClientPanel;
	public GameObject LobbyPanel;
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
			LobbyPanel.SetActive (false);
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
		case GameStatus.Lobby:
			LobbyPanel.SetActive (true);
			ServerPanel.SetActive (false);
			ClientPanel.SetActive (false);
			break;
		case GameStatus.Playing:
			LobbyPanel.SetActive (false);
			break;
		case GameStatus.GameOver:
			break;
		case GameStatus.Error:
			LobbyPanel.SetActive (false);
			ErrorPanel.SetActive (true);
			break;
		default:
			break;
		}
		gs = s;
	}
}
