using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;


/// <summary>
/// Zentrale Klasse zum Steuern der Multiplayer-Komponente, wird sowohl von den Clients, als auch vom Server bzw. Host verwendet
/// </summary>
public class ARNetworkManager : NetworkManager {

    /// <summary>
    /// Liste mit verbundenen Clients auf dem Server
    /// </summary>
    private List<ARClient> clientsOnServer = new List<ARClient>();

    /// <summary>
    /// Liste mit Verbindungen auf dem Client
    /// </summary>
    private List<ARClient> connectionsOnClient = new List<ARClient>();

    /// <summary>
    /// Callback, wenn Monsterdaten empfangen wurden (Client)
    /// </summary>
    public Action<MonsterDataMessage> OnMonsterDataReceived { get; set; }
    /// <summary>
    /// Callback, wenn Spielstart empfangen wurde (Client)
    /// </summary>
    public Action<long> OnStartGame { get; set; }
    /// <summary>
    /// Callback, wenn Spielerstatusupdate empfangen wurde (Client)
    /// </summary>
    public Action<PlayerStatusUpdateMessage> OnPlayerStatusUpdate;

    bool gameStarted = false;
    bool gameFinish = false;

    private DateTime startTime;
    private DateTime lastCheck;

    private NetworkClient cachedClient;

    private ARLobbyManager lobbyManager;
    /// <summary>
    /// Callback, wenn ein Spieler dem Match beigetreten ist (Client)
    /// </summary>
    internal Action<int> OnPlayerAdded;
    private Dictionary<int, PlayerStatus> playerStatusList;

    /// <summary>
    /// Erweiterung um eigene Message-Typen
    /// </summary>
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


    private void Awake()
    {
        lobbyManager = GetComponent<ARLobbyManager>();
        playerStatusList = new Dictionary<int, PlayerStatus>();

    }

    /// <summary>
    /// Punkteberechnung auf dem Server
    /// </summary>
    private void Update()
    {
        if(gameStarted && !gameFinish)
        {
            var now = DateTime.UtcNow;
            var secondsPlayed = now.Subtract(startTime).TotalSeconds;
            if(secondsPlayed >= 0)
            {
                if(now.Subtract(lastCheck).TotalSeconds > 10)
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
          

 
    }

    /// <summary>
    /// CLIENT
    /// 
    /// Senden einer Keep-Alive-Message
    /// 
    /// </summary>
    internal void SendKeepAliveMessage()
    {
        if(client!=null && client.isConnected)
        {
            client.Send(MyMsgType.KeepAlive, new KeepAliveMessage());
            Debug.Log("Send Keep Alive");
        }
       
    }

   
    /// <summary>
    /// CLIENT
    /// 
    /// Senden des Events, Monster wurde getötet, an den Server
    /// </summary>
    /// <param name="clientId"></param>
    public void SendMonsterKilledEvent(int clientId)
    {
        MonsterKilledMessage monsterKilledMessage = new MonsterKilledMessage();
        monsterKilledMessage.clientId = clientId;
        client.Send(MyMsgType.MonsterKilled, monsterKilledMessage);

    }

    /// <summary>
    /// CLIENT
    /// 
    /// Senden des Events, Spieler ist tot, an den Server
    /// 
    /// </summary>
    public void SendPlayerKilledEvent()
    {
        PlayerKilledMessage killMessage = new PlayerKilledMessage();
      
        client.Send(MyMsgType.PlayerKilled, killMessage);

    }


    /// <summary>
    /// CLIENT
    /// 
    /// Senden der eigenen Monsterpositionsdaten
    /// </summary>
    /// <param name="monsterData"></param>
    public void sendMonsterDataToServer(MonsterData[] monsterData)
    {
        MonsterDataMessage monsterDataMessage = new MonsterDataMessage();
        monsterDataMessage.monsterData = monsterData;
        if(client==null)
        {
            client = StartClient();
        }        
        client.Send(MyMsgType.MonsterDataSent, monsterDataMessage);       
    }



    /// <summary>
    /// Workaround zum Cachen des Clients, Bug in Unet?
    /// </summary>
    /// <param name="networkAddress"></param>
    /// <param name="networkPort"></param>
    internal void StartClientWithCache(string networkAddress, int networkPort)
    {
        this.networkAddress = networkAddress;
        this.networkPort = networkPort;

        cachedClient = this.StartClient();
    }


    /// <summary>
    /// SERVER
    /// 
    /// Handler für eingehende Nachrichten, dass ein Spieler gestorben ist
    /// </summary>
    /// <param name="netMsg"></param>
    public void OnServerPlayerKilledReceived(NetworkMessage netMsg)
    {
        var ps = playerStatusList[netMsg.conn.connectionId];
        ps.points -= 120;
    }

    /// <summary>
    /// SERVER
    /// 
    /// Handler für eingehende Nachrichten, dass ein Monster getötet wurde
    /// </summary>
    /// <param name="netMsg"></param>
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

    /// <summary>
    /// SERVER
    /// 
    /// Handler für eingehende Nachrichten mit Monsterpositionsdaten eines Spielers
    /// Verteilt die Daten an die anderen Spieler und überprüft, ob alle Spieler bereits ihre Daten gesendet haben,
    /// falls ja, wird das Spiel gestartet
    /// </summary>
    /// <param name="netMsg"></param>
    public void OnServerMonsterDataReceived(NetworkMessage netMsg)
    {
        MonsterDataMessage message = netMsg.ReadMessage<MonsterDataMessage>();
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




    /// <summary>
    /// SERVER
    /// 
    /// Handler für eingehende Verbindungen auf dem Server, d.h. ein Spieler ist dem Match beigetreten,
    /// es werden die passenden Handler für diese Verbindungen gesetzt und das Event wird an alle bereits verbundenen Spieler gesendet
    /// </summary>
    /// <param name="netMsg"></param>

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


    /// <summary>
    /// SERVER
    /// 
    /// Handler für eingehende Nachrichten, dass ein Client sich erfolgreich verbunden hat
    /// </summary>
    /// <param name="netMsg"></param>
    private void OnServerPlayerIsSuccessfullyConnected(NetworkMessage netMsg)
    {
        var msg = new ClientAddedMessage();
        msg.ClientConnectionId = netMsg.conn.connectionId;
        NetworkServer.SendToAll(MyMsgType.ClientAdded, msg);

        Debug.LogFormat("Server send Info: Player is connected - {0}", msg.ClientConnectionId);

    }

    private void OnServerPlayerKeepAlive(NetworkMessage netMsg)
    {
       //Spieler-Verbindung lebt
    }

   

    /// <summary>
    /// CLIENT
    /// 
    /// Handler für eingehende Nachrichten mit gegnerischen Monsterpositionsdaten
    /// </summary>
    /// <param name="netMsg"></param>
    public void OnClientMonsterDataReceived(NetworkMessage netMsg)
    {
        //Erhalt der gegnerischen Einheiten
        MonsterDataMessage message = netMsg.ReadMessage<MonsterDataMessage>();
        OnMonsterDataReceived(message);
    }


    /// <summary>
    /// CLIENT
    /// 
    /// Handler für das Event, dass sich der Client erfolgreich mit dem Server verbunden hat
    /// </summary>
    /// <param name="netMsg"></param>
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


    /// <summary>
    /// CLIENT
    /// 
    /// Handler für eingehende Nachrichten mit Playerstatusupdates
    /// </summary>
    /// <param name="netMsg"></param>
    private void OnClientPlayerStatusUpdate(NetworkMessage netMsg)
    {

        var updateMessage = netMsg.ReadMessage<PlayerStatusUpdateMessage>();
        OnPlayerStatusUpdate(updateMessage);
    }

    /// <summary>
    /// CLIENT
    /// 
    /// Handler für eingehende Nachrichten, dass ein Spiel gestartet wurde
    /// </summary>
    /// <param name="netMsg"></param>

    private void OnClientStartGame(NetworkMessage netMsg)
    {
        // Starte Platzierung und Game
        var msg = netMsg.ReadMessage<StartGameMessage> ();
        OnStartGame (msg.startTime);
    }

    /// <summary>
    /// CLIENT
    /// 
    /// Handler für eingehende Nachrichten, dass sich ein Spieler verbunden hat
    /// </summary>
    /// <param name="netMsg"></param>
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

    /// <summary>
    /// CLIENT
    /// 
    /// Handler, das ein Spieler bereit ist
    /// </summary>
    /// <param name="netMsg"></param>
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

