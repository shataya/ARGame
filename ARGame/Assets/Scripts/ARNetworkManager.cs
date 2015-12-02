using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;

public class ARNetworkManager : NetworkManager {


    private List<ARClient> clientsOnServer = new List<ARClient>();
    private List<ARClient> connectionsOnClient = new List<ARClient>();
    private ARLobbyManager lobbyManager;

    public Action<MonsterDataMessage> OnMonsterDataReceived { get; set; }

    public Action<DateTime> OnStartGame { get; set; }

    private void Awake()
    {
        lobbyManager = GetComponent<ARLobbyManager>();


    }

    public class MyMsgType
    {
        public static short ClientAdded = MsgType.Highest + 1;
        public static short ClientReady = MsgType.Highest + 2;
        public static short MonsterDataSent = MsgType.Highest + 3;
        public static short StartGame = MsgType.Highest + 4;
    };


    public void sendMonsterDataToServer(List<MonsterData> monsterData)
    {
        MonsterDataMessage monsterDataMessage = new MonsterDataMessage();
        monsterDataMessage.monsterData = monsterData;
        client.Send(MyMsgType.MonsterDataSent, monsterDataMessage);
       
    }

    public void OnServerMonsterDataReceived(NetworkMessage netMsg)
    {
        MonsterDataMessage message = netMsg.ReadMessage<MonsterDataMessage>();
        message.clientId = netMsg.conn.connectionId;

        foreach (var client in clientsOnServer)
        {
            if(client.ClientConnectionId!=message.clientId)
            {
                NetworkServer.SendToClient(client.ClientConnectionId, MyMsgType.MonsterDataSent, message);
            } else
            {
                client.Ready = true;
            }
          
        }

        if(checkIfAllAreReady())
        {
            // Send Start Game
            var startMsg = new StartGameMessage();
            startMsg.startTime = DateTime.Now.AddSeconds(60);
            NetworkServer.SendToAll(MyMsgType.StartGame,startMsg);
        }

    }

    private bool checkIfAllAreReady()
    {
        bool allReady = true;

        foreach (var client in clientsOnServer)
        {
            allReady = client.Ready;
        }

        return allReady;
    }

    public void OnClientMonsterDataReceived(NetworkMessage netMsg)
    {
        //Erhalt der gegnerischen Einheiten
        MonsterDataMessage message = netMsg.ReadMessage<MonsterDataMessage>();
        
        ///Speichern der gegnerischen Monsterdata
    }
 

    public override void OnServerConnect(NetworkConnection conn)
    {
        // New Player connected
        base.OnServerConnect(conn);

        foreach(var client in clientsOnServer)
        {
            var tmpmsg = new ClientAddedMessage();
            tmpmsg.ClientConnectionId = client.ClientConnectionId;
            NetworkServer.SendToClient(conn.connectionId, MyMsgType.ClientAdded, tmpmsg);
        }

     
        var arclient = new ARClient();
        arclient.ClientConnectionId = conn.connectionId;
        arclient.Ready = false;
    
        clientsOnServer.Add(arclient);


        conn.RegisterHandler(MyMsgType.MonsterDataSent, OnServerMonsterDataReceived);

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
        conn.RegisterHandler(MyMsgType.StartGame, OnClientStartGame);
        conn.RegisterHandler(MyMsgType.MonsterDataSent, OnClientMonsterDataReceived);
        Debug.LogFormat("Client: Player is conected - {0}", conn.connectionId);

    }

    private void OnClientStartGame(NetworkMessage netMsg)
    {
        /// Starte Platzierung und Game
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

