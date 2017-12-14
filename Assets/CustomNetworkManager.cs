using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class MsgTypes
{
	public const short PlayerPrefabSelect = MsgType.Highest + 1;

	public class PlayerPrefabMsg : MessageBase
	{
		public short controllerID;
		public short prefabIndex;
	}
}

public class CustomNetworkManager : NetworkManager {

	public short playerPrefabIndex;

	public int selGridInt = 0;
	public string[] selStrings = new string[] { "Princess", "Brutius", "Jane", "Funkey"};

	void OnGUI()
	{
		if(!isNetworkActive)
		{
			selGridInt = GUI.SelectionGrid(new Rect(Screen.width - 200, 10, 200, 50), selGridInt, selStrings, 2);
			playerPrefabIndex = (short) (selGridInt + 1);
		}	
	}

	public override void OnStartServer()
	{
		NetworkServer.RegisterHandler(MsgTypes.PlayerPrefabSelect, OnResponsePrefab);
		base.OnStartServer();
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		client.RegisterHandler(MsgTypes.PlayerPrefabSelect, OnRequestPrefab);
		base.OnClientConnect(conn); 
	}

	private void OnRequestPrefab(NetworkMessage netMsg)
	{
		MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
		msg.controllerID = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>().controllerID;
		msg.prefabIndex = playerPrefabIndex;
		client.Send(MsgTypes.PlayerPrefabSelect, msg);
	}

	private void OnResponsePrefab(NetworkMessage netMsg)
	{
		MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();
		playerPrefab = spawnPrefabs[msg.prefabIndex];
		base.OnServerAddPlayer(netMsg.conn, msg.controllerID); 
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
		msg.controllerID = playerControllerId;
		NetworkServer.SendToClient(conn.connectionId, MsgTypes.PlayerPrefabSelect, msg);
	}

	public void SwitchPlayer(SetupLocalPlayer player, int cid)
	{
		GameObject newPlayer = Instantiate(spawnPrefabs[cid],
								player.gameObject.transform.position,
								player.gameObject.transform.rotation);
		playerPrefab = spawnPrefabs[cid];
		Destroy(player.gameObject);
		NetworkServer.ReplacePlayerForConnection(player.connectionToClient, newPlayer,0); 

	}

}
