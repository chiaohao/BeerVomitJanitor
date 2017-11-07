using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayerClientController : NetworkLobbyPlayer {
	//controllers
	SystemController sc;
	NetworkController nc;

	//UI
	public GameObject waitingText;
	public GameObject readyText;
	public GameObject readyBtn;

	//sync
	[SyncVar]
	public bool isReady;

	void Awake(){
		sc = FindObjectOfType<SystemController> ();
		nc = FindObjectOfType<NetworkController> ();
	}

	public void SwitchReady(){
		readyToBegin = readyToBegin ? false : true;
		isReady = readyToBegin;
		if (readyToBegin)
			SendReadyToBeginMessage ();
		else
			SendNotReadyToBeginMessage ();
	}

	public override void OnClientEnterLobby ()
	{
		base.OnClientEnterLobby ();
		sc.setupLobbyPlayer (gameObject, isServer);
	}

	void Update(){
		isReady = readyToBegin;
		readyText.SetActive (isReady);
		waitingText.SetActive (!isReady);
		if (isLocalPlayer)
			UpdateLocalPlayer ();
		else
			UpdateOtherPlayers ();
	}

	private void UpdateLocalPlayer(){
		if (!isServer)
			readyBtn.SetActive (true);
	}

	private void UpdateOtherPlayers(){
		readyBtn.SetActive (false);
	}
}
