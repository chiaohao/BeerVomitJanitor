using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkController : NetworkLobbyManager {
	public ServerDataController sdc;
	public GameStatusController gsc;

	public override void OnLobbyServerDisconnect (NetworkConnection conn)
	{
		base.OnLobbyServerDisconnect (conn);
	}

	public override void OnLobbyServerConnect (NetworkConnection conn)
	{
		base.OnLobbyServerConnect (conn);
		sdc.AddPlayer (conn.connectionId, sdc.players.Count == 0 ? 1 : 0);
	}
		
	public override void OnClientDisconnect (NetworkConnection conn)
	{
		base.OnClientDisconnect (conn);
		gsc.switchStatus (GameStatus.Error);

	}

	public override void OnLobbyClientDisconnect (NetworkConnection conn)
	{
		base.OnLobbyClientDisconnect (conn);
		gsc.switchStatus (GameStatus.Error);
	}

	public override void OnClientSceneChanged (NetworkConnection conn)
	{
		base.OnClientSceneChanged (conn);
		if (SceneManager.GetActiveScene ().name == playScene)
			gsc.switchStatus (GameStatus.Playing);
	}

}
