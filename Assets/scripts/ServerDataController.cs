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

	public bool isGameStart;

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

	[SyncVar]
	public float dirtyLevel;

	void Awake(){
		gsc = FindObjectOfType<GameStatusController> ();
		nc = FindObjectOfType<NetworkController> ();

		dirtyLevel = 0f;
		isGameStart = false;;
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
		if (gameTime > 0f) {
			isGameStart = true;
		}
		gameTime -= Time.deltaTime;
		if (gameTime < 0f)  {
			gameTime = 0f;
			if (isGameStart) {
				isGameStart = false;
				if (dirtyLevel >= 0.2f) {
					FindObjectOfType<GameUIController> ().SetWinPanel (true);
				} 
				else {
					FindObjectOfType<GameUIController> ().SetWinPanel (false);
				}
			}
		}
	}
}
