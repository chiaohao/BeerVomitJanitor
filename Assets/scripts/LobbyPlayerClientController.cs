using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayerClientController : NetworkLobbyPlayer {
	//controllers
	SystemController sc;
	NetworkController nc;
	ServerDataController sdc;

	//UI
	public GameObject waitingText;
	public GameObject readyText;
	public Image head;

	public Sprite Drunker;
	public Sprite Cleaner;

	//sync
	[SyncVar]
	public bool isReady;

	[SyncVar]
	int characterId;

	void Awake(){
		sc = FindObjectOfType<SystemController> ();
		nc = FindObjectOfType<NetworkController> ();
		sdc = FindObjectOfType<ServerDataController> ();
	}

	void Start(){
		if (isLocalPlayer) {
			foreach (ServerDataController.PlayerAttribute p in sdc.players) {
				if (p.NetworkId == connectionToServer.connectionId)
					characterId = p.CharacterId;
			}
		} 
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
		if (characterId == 1) {
			head.sprite = Cleaner;
		} 
		else if (characterId == 0) {
			head.sprite = Drunker;
		}

		transform.localScale = Vector3.one;
		isReady = readyToBegin;
		readyText.SetActive (isReady);
		waitingText.SetActive (!isReady);
		if (isLocalPlayer)
			UpdateLocalPlayer ();
	}

	private void UpdateLocalPlayer(){
		Button startBtn = FindObjectOfType<SystemController> ().gameStartBtn.GetComponent<Button> ();
		Button readyBtn = FindObjectOfType<SystemController> ().readyBtn.GetComponent<Button> ();
		string ip, port;
		if (!isServer) {
			startBtn.gameObject.SetActive (false);
			readyBtn.gameObject.SetActive (true);
			readyBtn.onClick.RemoveAllListeners ();
			readyBtn.onClick.AddListener (SwitchReady);
			ip = nc.networkAddress;
			port = nc.networkPort.ToString();
		} 
		else {
			startBtn.gameObject.SetActive (true);
			readyBtn.gameObject.SetActive (false);
			readyText.SetActive (false);
			waitingText.SetActive (false);
			ip = Network.player.ipAddress;
			port = nc.networkPort.ToString();
		}
		FindObjectOfType<GameStatusController> ().UpdateLobbyUI (ip, port);
	}
		
}
