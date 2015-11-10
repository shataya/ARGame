using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ARNetworkManager : NetworkManager {

    public override void OnServerConnect(NetworkConnection conn)
    {
        // New Player connected
        base.OnServerConnect(conn);
        conn.RegisterHandler(MsgType.Ready, OnServerReady);
        Debug.LogFormat("Server: Player is added - {0}", conn.connectionId);

    }

    private void OnServerReady(NetworkMessage netMsg)
    {
        // Player is ready

        Debug.LogFormat("Server: Player is ready - {0}", netMsg.conn.connectionId);
    }


    public override void OnClientConnect(NetworkConnection conn)
    {
        // With Server connected
        base.OnClientConnect(conn);
        conn.RegisterHandler(MsgType.Ready, OnClientReady);
        Debug.LogFormat("Client: Player is added - {0}", conn.connectionId);

    }


    private void OnClientReady(NetworkMessage netMsg)
    {
        // Player is ready
      
        Debug.LogFormat("Client: Player is ready - {0}", netMsg.conn.connectionId);
    }





}
