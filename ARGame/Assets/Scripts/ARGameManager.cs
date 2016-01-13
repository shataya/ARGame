using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class ARGameManager : MonoBehaviour
{
    public GameObject startBlock;
    public GameObject lobbyBlock;
    public GameObject floorBlock;
 
    public Text counter;
    public Text ownPoints;
    public Text ownLifes;
    public Text enemyPoints;
    public Text enemyLifes;
    public GameObject wonMessage;
    public GameObject lostMessage;
    public GameObject player;
    public GameObject hud;

    public DateTime startTime;

    public bool started = false;

    private int enemyClientId;
    private DateTime lastCheck;
    private bool joined = false;

    private Dictionary<int, PlayerStatus> playerStatusList;

    


    // Use this for initialization
    void Start()
    {
        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
        nd.Initialize();
        var ml = player.GetComponent<MonsterLauncher>();
        ml.OnMonsterDied = OnEnemyMonsterDied;

        playerStatusList = new Dictionary<int, PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if(started)
        {
            var timetostart = startTime.Subtract(DateTime.UtcNow).TotalSeconds;
            Debug.LogFormat("Time to Start: {0}", timetostart);
            counter.text = Mathf.CeilToInt((float)timetostart).ToString();

            if(timetostart<=0)
            {
                //STARTEN
                started = false;
                counter.gameObject.SetActive(false);
                var ml = player.GetComponent<MonsterLauncher>();
                ml.ActivateEnemies();
               // transform.position = GetComponent<Player> ().spawnPoint;
            }
        }

        if(joined)
        {
            var now = DateTime.UtcNow;

            if (now.Subtract(lastCheck).TotalSeconds > 15)
            {
                lastCheck = now;
                var nm = this.gameObject.GetComponent<ARNetworkManager>();
                nm.SendKeepAliveMessage();                
            }
        }
    
           
    }

    public void OnEnemyMonsterDied(int clientId)
    {
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
        nm.SendMonsterKilledEvent(clientId);
    }

    public void startMatch()
    {
        Debug.Log("Start Match");
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
        nm.logLevel = LogFilter.FilterLevel.Info;
        nm.networkPort = 7777;
      
        nm.OnStartGame = startGame;
        nm.OnMonsterDataReceived = saveMonsterData;
        nm.OnPlayerAdded = AddPlayerToMatch;
        nm.OnPlayerStatusUpdate = updatePlayerStatus;

        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
       

        startBlock.SetActive(false);
        lobbyBlock.SetActive(true);
        nm.StartHost();
        nd.StartAsServer();
        joined = true;

    }

   
    public void findMatches()
    {        
        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
        nd.Initialize();
        nd.StartAsClient();
    }


    public void joinMatch(int match)
    {
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
       
        var networkAddress = GetComponent<ARNetworkDiscoveryHandler>().DropdownMatches.options[match].text;
        var networkPort = 7777;
        nm.StartClientWithCache(networkAddress, networkPort);
       
        startBlock.SetActive(false);
        lobbyBlock.SetActive(true);
        nm.OnStartGame = startGame;
        nm.OnPlayerAdded = AddPlayerToMatch;
        nm.OnMonsterDataReceived = saveMonsterData;
        nm.OnPlayerStatusUpdate = updatePlayerStatus;
        joined = true;
    }

    public void backToLobby()
    {
        lobbyBlock.SetActive(true);
        floorBlock.SetActive(false);
      
    }

    public void placeSoldiers()
    {
        lobbyBlock.SetActive(false);
        floorBlock.SetActive(true);
       
    }

    public void AddPlayerToMatch(int clientId)
    {
        if(!playerStatusList.ContainsKey(clientId))
        {
            playerStatusList.Add(clientId, new PlayerStatus());
        }
       
    }

    public void sendMonsterData()
    {
        var ml = player.GetComponent<MonsterLauncher>();
        var nm = this.gameObject.GetComponent<ARNetworkManager>();

        var mlArr = ml.MonsterDataList;
        if(mlArr.Length==4)
        {
            nm.sendMonsterDataToServer(mlArr);

            startBlock.SetActive(false);
            lobbyBlock.SetActive(false);
           
        }
       
    }

    public void startGame(long startTime)
    {
        if(!started)
        {
            Debug.LogFormat("Starte Game mit Counter {0}", startTime);
            counter.gameObject.SetActive(true);
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            this.startTime = dtDateTime.AddSeconds(startTime).ToUniversalTime();


            this.started = true;

            hud.SetActive(true);
        }
      
    }

    public void saveMonsterData(MonsterDataMessage monsterDataMessage)
    {
        var ml = player.GetComponent<MonsterLauncher>();
        enemyClientId = monsterDataMessage.clientId;
        ml.SetEnemies(monsterDataMessage.clientId, monsterDataMessage.monsterData);
    }

    private void updatePlayerStatus(PlayerStatusUpdateMessage message)
    {
        var ps = playerStatusList[message.clientId];
        ps.lifes = message.lifes;
        ps.points = message.points;

        if(message.clientId==enemyClientId)
        {
            enemyLifes.text = String.Format("{0} / 4", ps.lifes);
            enemyPoints.text = String.Format("{0} / 2400", ps.points);
            if(ps.lifes==0)
            {
                wonMessage.SetActive(true);
            }
            if(ps.points >= 2400)
            {
                lostMessage.SetActive(true);
            }
        } else
        {
            ownLifes.text = String.Format("{0} / 4", ps.lifes);
            ownPoints.text = String.Format("{0} / 2400", ps.points);
            if (ps.lifes == 0)
            {
                lostMessage.SetActive(true);
            }
            if(ps.points >= 2400)
            {
                wonMessage.SetActive(true);
            }
        }
    }


    public void playerDie()
    {
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
        nm.SendPlayerKilledEvent();
    }
}
