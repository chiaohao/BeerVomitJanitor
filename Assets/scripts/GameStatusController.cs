using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public GameObject BackgroundPanel;
	public GameObject MainPanel;
	public GameObject ServerPanel;
	public GameObject ClientPanel;
	public GameObject LobbyPanel;
	public Text LobbyIp;
	public Text LobbyPort;
	public GameObject ErrorPanel;

	[HideInInspector]
	public GameStatus gs;

	void Start () {
		gs = GameStatus.MainMenu;
	}
	
	public void switchStatus(GameStatus s){
		switch (s) {
		case GameStatus.MainMenu:
			BackgroundPanel.SetActive (true);
			MainPanel.SetActive (true);
			ServerPanel.SetActive (false);
			ClientPanel.SetActive (false);
			LobbyPanel.SetActive (false);
			ErrorPanel.SetActive (false);
			break;
		case GameStatus.HostCreating:
			BackgroundPanel.SetActive (true);
			MainPanel.SetActive (true);
			ServerPanel.SetActive (true);
			break;
		case GameStatus.ClientConnecting:
			BackgroundPanel.SetActive (true);
			MainPanel.SetActive (true);
			ClientPanel.SetActive (true);
			break;
		case GameStatus.Lobby:
			BackgroundPanel.SetActive (true);
			MainPanel.SetActive (true);
			LobbyPanel.SetActive (true);
			ServerPanel.SetActive (false);
			ClientPanel.SetActive (false);
			break;
		case GameStatus.Playing:
			BackgroundPanel.SetActive (false);
			MainPanel.SetActive (false);
			LobbyPanel.SetActive (false);
			break;
		case GameStatus.GameOver:
			BackgroundPanel.SetActive (true);
			MainPanel.SetActive (true);
			break;
		case GameStatus.Error:
			BackgroundPanel.SetActive (true);
			MainPanel.SetActive (true);
			LobbyPanel.SetActive (false);
			ErrorPanel.SetActive (true);
			break;
		default:
			break;
		}
		gs = s;
	}

	public void UpdateLobbyUI(string ip, string port){
		LobbyIp.text = ip;
		LobbyPort.text = port;
	}
}
