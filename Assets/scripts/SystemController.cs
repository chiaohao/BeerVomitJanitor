using System.Collections;
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

	void Start () {
		nc = FindObjectOfType<NetworkController> ();
		gsc = GetComponent<GameStatusController> ();
		sdc = FindObjectOfType<ServerDataController> ();
		hostIpText.text = "IP: " + Network.player.ipAddress;
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
		gsc.switchStatus (GameStatus.Waiting);
	}

	public void onClientConnectBtnPressed(){
		nc.networkAddress = clientIpText.text;
		nc.networkPort = Convert.ToInt32(clientPortText.text);
		nc.StartClient ();
		gsc.switchStatus (GameStatus.Waiting);
	}

	public void onGameStartPressed(){
		if(sdc == null)
			sdc = FindObjectOfType<ServerDataController> ();
		sdc.RpcGameStart ();
	}

	public void onBackToTitlePressed(){
		gsc.switchStatus (GameStatus.MainMenu);
	}
}
