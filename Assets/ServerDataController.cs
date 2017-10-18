using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ServerDataController : NetworkBehaviour {

	// Controllers
	GameStatusController gsc;
	NetworkController nc;

	//SyncVars
	[SyncVar(hook = "UpdatePlayerNumText")]
	[HideInInspector]
	public int playerNum = 0; //syncval example
	public Text playerNumText;

	[SyncVar]
	public float gameTime;

	void Awake(){
		gsc = FindObjectOfType<GameStatusController> ();
		nc = FindObjectOfType<NetworkController> ();
		UpdatePlayerNumText (playerNum);
	}

	public void UpdatePlayerNumText(int pn){
		//playerNumText.text = "Now Players: " + pn.ToString ();
	}

	[ClientRpc]
	public void RpcGameStart(){
		gsc.switchStatus (GameStatus.CharacterChoose);
	}

	void Update(){
		playerNumText.text = "Now Players: " + nc.numPlayers.ToString ();
	}
}
