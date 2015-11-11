using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;

public class ARNetworkManager : NetworkManager {

    private List<NetworkConnection> connectionsOnServer = new List<NetworkConnection>();
    private List<ARClient> connectionsOnClient = new List<ARClient>();
    private ARLobbyManager lobbyManager;

    private void Awake()
    {
        lobbyManager = GetComponent<ARLobbyManager>();
    }

    public class MyMsgType
    {
        public static short ClientAdded = MsgType.Highest + 1;
        public static short ClientReady = MsgType.Highest + 2;
    };

    public override void OnServerConnect(NetworkConnection conn)
    {
        // New Player connected
        base.OnServerConnect(conn);

        foreach(var client in connectionsOnServer)
        {
            var tmpmsg = new ClientAddedMessage();
            tmpmsg.ClientConnectionId = conn.connectionId;
            NetworkServer.SendToClient(conn.connectionId, MyMsgType.ClientAdded, tmpmsg);
        }

        connectionsOnServer.Add(conn);
        var msg = new ClientAddedMessage();
        msg.ClientConnectionId = conn.connectionId;
        NetworkServer.SendToAll(MyMsgType.ClientAdded, msg);
   
        Debug.LogFormat("Server: Player is connected - {0}", msg.ClientConnectionId);

    }

    private void OnServerReady(NetworkMessage netMsg)
    {
        // Player is ready
        var msg = new ClientReadyMessage();
        msg.ClientConnectionId = netMsg.conn.connectionId;
        NetworkServer.SendToAll(MyMsgType.ClientReady, msg);
        Debug.LogFormat("Server: Player is ready - {0}", msg.ClientConnectionId);
    }


    public override void OnClientConnect(NetworkConnection conn)
    {
        // With Server connected
        base.OnClientConnect(conn);
        conn.RegisterHandler(MyMsgType.ClientAdded, OnClientAdded);
        conn.RegisterHandler(MyMsgType.ClientReady, OnClientReady);
        Debug.LogFormat("Client: Player is conected - {0}", conn.connectionId);

    }

    public void OnClientAdded(NetworkMessage netMsg)
    {
        // Player is added
        var msg = netMsg.ReadMessage<ClientAddedMessage>();
        var arclient = new ARClient();
        arclient.ClientConnectionId = msg.ClientConnectionId;
        arclient.Ready = false;       
      
        connectionsOnClient.Add(arclient);
        lobbyManager.AddPlayer(arclient);
        Debug.LogFormat("Client: Player is added - {0}", arclient.ClientConnectionId);

    }


    private void OnClientReady(NetworkMessage netMsg)
    {
        // Player is ready
        var msg = netMsg.ReadMessage<ClientReadyMessage>();
        var arClient = connectionsOnClient.FirstOrDefault(client => client.ClientConnectionId == msg.ClientConnectionId);
        if(arClient!=null)
        {
            arClient.Ready = true;
        }
      
        Debug.LogFormat("Client: Player is ready - {0}", netMsg.conn.connectionId);
    }





}

