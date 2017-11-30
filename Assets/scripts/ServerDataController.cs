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
	[SyncVar]
	public float gameTime;

	void Awake(){
		gsc = FindObjectOfType<GameStatusController> ();
		nc = FindObjectOfType<NetworkController> ();
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
		gameTime -= Time.deltaTime;
		if (gameTime < 0f)
			gameTime = 0f;
	}
}
