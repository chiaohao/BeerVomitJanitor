using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : NetworkManager {

	public ServerDataController sdc;
	public GameStatusController gsc;

	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) {
		base.OnServerAddPlayer (conn, playerControllerId);
		sdc.playerNum += 1;
	}

	public override void OnServerDisconnect (NetworkConnection conn){
		base.OnServerDisconnect (conn);
		sdc.playerNum -= 1;
	}

	public override void OnClientDisconnect (NetworkConnection conn)
	{
		base.OnClientDisconnect (conn);
		gsc.switchStatus (GameStatus.Error);

	}

}
