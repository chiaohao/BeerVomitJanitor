﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SystemController : MonoBehaviour {

	NetworkController nc;
	GameStatusController gsc;
	ServerDataController sdc;

	//UI
	public Text hostIpText;
	public InputField hostPortText;

	public InputField clientIpText;
	public InputField clientPortText;

	public GameObject lobbyPlayerList;
	public GameObject gameStartBtn;
	public GameObject readyBtn;

	public float gametime = 5f * 60f;

	void Start () {
		nc = FindObjectOfType<NetworkController> ();
		gsc = GetComponent<GameStatusController> ();
		hostIpText.text = Network.player.ipAddress;
	}

	public void onCreateRoomPressed(){
		gsc.switchStatus (GameStatus.HostCreating);
	}

	public void onJoinRoomPressed(){
		gsc.switchStatus (GameStatus.ClientConnecting);
	}

	public void onCreateHostBtnPressed(){
		nc.networkAddress = Network.player.ipAddress;
		nc.networkPort = Convert.ToInt32(hostPortText.text);
		nc.StartHost ();
		gsc.switchStatus (GameStatus.Lobby);
	}

	public void onClientConnectBtnPressed(){
		nc.networkAddress = clientIpText.text;
		nc.networkPort = Convert.ToInt32(clientPortText.text);
		nc.StartClient ();
		gsc.switchStatus (GameStatus.Lobby);
	}

	public void onGameStartPressed(){
		if (nc.numPlayers <= 1)
			return;
		sdc = FindObjectOfType<ServerDataController> ();
		sdc.gameTime = gametime;
		LobbyPlayerClientController[] lpccs = FindObjectsOfType<LobbyPlayerClientController> ();
		LobbyPlayerClientController lpcc_local = null;
		foreach (LobbyPlayerClientController lpcc in lpccs) {
			if (lpcc.isLocalPlayer)
				lpcc_local = lpcc;
			else if (!lpcc.isReady)
				return;
		}
		lpcc_local.SendReadyToBeginMessage ();
	}

	public void onBackToTitlePressed(){
		gsc.switchStatus (GameStatus.MainMenu);
	}

	public void setupLobbyPlayer(GameObject lobbyPlayer, bool isServer){
		lobbyPlayer.transform.SetParent (lobbyPlayerList.transform);
		gameStartBtn.SetActive(isServer ? true : false);
	}

	public void onExitGamePressed(){
		Application.Quit ();
	}
}
