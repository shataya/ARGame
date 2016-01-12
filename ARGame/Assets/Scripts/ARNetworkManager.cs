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
    internal Action<int> OnPlayerAdded;
    private Dictionary<int, PlayerStatus> playerStatusList;

    public Action<MonsterDataMessage> OnMonsterDataReceived { get; set; }

    public Action<long> OnStartGame { get; set; }

    public Action<PlayerStatusUpdateMessage> OnPlayerStatusUpdate;

    bool gameStarted = false;
    private DateTime startTime;
    private DateTime lastCheck;
    bool gameFinish = false;

    private void Awake()
    {
        lobbyManager = GetComponent<ARLobbyManager>();
        playerStatusList = new Dictionary<int, PlayerStatus>();

    }

    private void Update()
    {
        if(gameStarted && !gameFinish)
        {
            var now = DateTime.UtcNow;
            var secondsPlayed = now.Subtract(startTime).TotalSeconds;
            if(secondsPlayed >= 0)
            {
                if(lastCheck==null)
                {
                    lastCheck = now;
                } else if(now.Subtract(lastCheck).TotalSeconds > 10)
                {
                    lastCheck = now;
                    foreach(KeyValuePair<int, PlayerStatus> pair in playerStatusList)
                    {
                        pair.Value.points += pair.Value.lifes * 15;
                        var ps = new PlayerStatusUpdateMessage();
                        ps.lifes = pair.Value.lifes;
                        ps.points = pair.Value.points;
                        ps.clientId = pair.Key;
                        if(ps.points >= 2400)
                        {
                            gameFinish = true;
                        }
                        NetworkServer.SendToAll(MyMsgType.PlayerStatusUpdate, ps);
                    }

                   

                }
            } 
          
        }
            //1 Punkt pro Sekunde, pro Gegner

 
    }

    internal void SendKeepAliveMessage()
    {
        if(client!=null && client.isConnected)
        {
            client.Send(MyMsgType.KeepAlive, new KeepAliveMessage());
            Debug.Log("Send Keep Alive");
        }
       
    }

    public class MyMsgType
    {
        public static short ClientAdded = MsgType.Highest + 1;
        public static short ClientReady = MsgType.Highest + 2;
        public static short MonsterDataSent = MsgType.Highest + 3;
        public static short StartGame = MsgType.Highest + 4;
        public static short MonsterKilled = MsgType.Highest + 5; 
        public static short PlayerStatusUpdate = MsgType.Highest + 6;
        public static short PlayerKilled = MsgType.Highest + 7;
         public static short KeepAlive = MsgType.Highest + 8;
};


    public void SendMonsterKilledEvent(int clientId)
    {
        MonsterKilledMessage monsterKilledMessage = new MonsterKilledMessage();
        monsterKilledMessage.clientId = clientId;
        client.Send(MyMsgType.MonsterKilled, monsterKilledMessage);

    }


    public void SendPlayerKilledEvent()
    {
        PlayerKilledMessage killMessage = new PlayerKilledMessage();
      
        client.Send(MyMsgType.PlayerKilled, killMessage);

    }

    public void sendMonsterDataToServer(MonsterData[] monsterData)
    {
        MonsterDataMessage monsterDataMessage = new MonsterDataMessage();
        monsterDataMessage.monsterData = monsterData;        
        client.Send(MyMsgType.MonsterDataSent, monsterDataMessage);       
    }

    public void OnServerPlayerKilledReceived(NetworkMessage netMsg)
    {
        PlayerKilledMessage message = netMsg.ReadMessage<PlayerKilledMessage>();
        var ps = playerStatusList[netMsg.conn.connectionId];
        ps.points -= 120;
    }
    public void OnServerMonsterKilledReceived(NetworkMessage netMsg)
    {
        MonsterKilledMessage message = netMsg.ReadMessage<MonsterKilledMessage>();
        var ps = playerStatusList[message.clientId];
        ps.lifes -= 1;
        if(ps.lifes == 0)
        {
            ps.points = -1000;
            gameFinish = true;
        }

        var updateMessage = new PlayerStatusUpdateMessage();
        updateMessage.clientId = message.clientId;
        updateMessage.lifes = ps.lifes;
        updateMessage.points = ps.points;

        NetworkServer.SendToAll(MyMsgType.PlayerStatusUpdate, updateMessage);
    }

    public void OnServerMonsterDataReceived(NetworkMessage netMsg)
    {
        MonsterDataMessage message = netMsg.ReadMessage<MonsterDataMessage>();
        Debug.Log (message == null ? "msg on srv is null" : (message.monsterData == null ? "monsterData null bei serverRec" : message.monsterData.Length.ToString ()));
        message.clientId = netMsg.conn.connectionId;

        foreach (var client in clientsOnServer)
        {
            if(client.ClientConnectionId!=message.clientId)
            {
                Debug.Log ("Schicke an Client: " + client.ClientConnectionId);
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
            var tmp = DateTime.UtcNow.AddSeconds(30);
            var timeSpan = (tmp - new DateTime(1970, 1, 1, 0, 0, 0));
            startMsg.startTime =(long)timeSpan.TotalSeconds;
            startTime = tmp;
            NetworkServer.SendToAll(MyMsgType.StartGame,startMsg);
            Debug.Log("Starte Game");
            gameStarted = true;
        }

    }

   

    private bool checkIfAllAreReady()
    {
        foreach (var client in clientsOnServer)
        {
            if (!client.Ready)
                return false;    
        }
        return true;
    }


   

    public void OnClientMonsterDataReceived(NetworkMessage netMsg)
    {
        //Erhalt der gegnerischen Einheiten
        MonsterDataMessage message = netMsg.ReadMessage<MonsterDataMessage>();
        Debug.Log (message == null ? "msg on client is null" : (message.monsterData == null ? "monsterData null bei clientRec" : message.monsterData.Length.ToString()));
        OnMonsterDataReceived (message);
    }
 

    public override void OnServerConnect(NetworkConnection conn)
    {
        // New Player connected
        base.OnServerConnect(conn);
        if(!playerStatusList.ContainsKey(conn.connectionId))
        {
            foreach (var client in clientsOnServer)
            {
                Debug.Log("Schicke bereits verbundene Clients!");
                var tmpmsg = new ClientAddedMessage();
                tmpmsg.ClientConnectionId = client.ClientConnectionId;
                NetworkServer.SendToClient(conn.connectionId, MyMsgType.ClientAdded, tmpmsg);
            }


            var arclient = new ARClient();
            arclient.ClientConnectionId = conn.connectionId;
            arclient.Ready = false;

            clientsOnServer.Add(arclient);
            playerStatusList.Add(conn.connectionId, new PlayerStatus());

            conn.RegisterHandler(MyMsgType.MonsterDataSent, OnServerMonsterDataReceived);
            conn.RegisterHandler(MyMsgType.MonsterKilled, OnServerMonsterKilledReceived);
            conn.RegisterHandler(MyMsgType.PlayerKilled, OnServerPlayerKilledReceived);
            Debug.Log("Set Keep Alive Handler");
            conn.RegisterHandler(MyMsgType.KeepAlive, OnServerPlayerKeepAlive);
            conn.RegisterHandler(MyMsgType.ClientAdded, OnServerPlayerIsSuccessfullyConnected);

            Debug.LogFormat("Ist Connected? {0}", conn.isConnected);



            Debug.LogFormat("Server: Player is connected - {0}", conn.connectionId);
        } 
       
    

    }

    private void OnServerPlayerIsSuccessfullyConnected(NetworkMessage netMsg)
    {
        var msg = new ClientAddedMessage();
        msg.ClientConnectionId = netMsg.conn.connectionId;
        NetworkServer.SendToAll(MyMsgType.ClientAdded, msg);

        Debug.LogFormat("Server send Info: Player is connected - {0}", msg.ClientConnectionId);

    }

    private void OnServerPlayerKeepAlive(NetworkMessage netMsg)
    {
       //SPieler lebt
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
        conn.RegisterHandler(MyMsgType.PlayerStatusUpdate, OnClientPlayerStatusUpdate);
      
        Debug.LogFormat("Client: Player is conected - {0}", conn.connectionId);
     

        client.Send(MyMsgType.ClientAdded, new ClientAddedMessage());
    }

    private void OnClientPlayerStatusUpdate(NetworkMessage netMsg)
    {

        var updateMessage = netMsg.ReadMessage<PlayerStatusUpdateMessage>();
        OnPlayerStatusUpdate(updateMessage);
    }

    private void OnClientStartGame(NetworkMessage netMsg)
    {
        // Starte Platzierung und Game
        var msg = netMsg.ReadMessage<StartGameMessage> ();
        OnStartGame (msg.startTime);
    }

    public void OnClientAdded(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ClientAddedMessage>();
        var arclient = new ARClient();
        arclient.ClientConnectionId = msg.ClientConnectionId;
        arclient.Ready = false;
        bool already = false;
        // Player is added
        foreach (var tmpclient in connectionsOnClient)
        {
            if(tmpclient.ClientConnectionId == arclient.ClientConnectionId)
            {
                already = true;
            }
        }
       
        if(!already)
        {
            Debug.LogFormat("Client: Player is added - {0}", arclient.ClientConnectionId);


            connectionsOnClient.Add(arclient);
            lobbyManager.AddPlayer(arclient);
            OnPlayerAdded(msg.ClientConnectionId);
        }
      
        
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

