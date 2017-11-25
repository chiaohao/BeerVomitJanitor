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

	//SyncLists
	public struct PlayerAttribute
	{
		public int NetworkId;
		public int CharacterId;
	}

	public class Players : SyncListStruct<PlayerAttribute>{};
	public Players players = new Players ();

	//SyncVars
	[SyncVar(hook = "UpdatePlayerNumText")]
	[HideInInspector]
	public int playerNum = 0; //syncval example

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

	public void AddPlayer(int Nid, int Cid){
		PlayerAttribute p = new PlayerAttribute ();
		p.NetworkId = Nid;
		p.CharacterId = Cid;
		players.Add (p);
	}

	void Update(){
		//Debug.Log (players.Count);
		//Debug.Log (players[0].NetworkId.ToString() + " " + players[0].CharacterId.ToString());
	}
}
