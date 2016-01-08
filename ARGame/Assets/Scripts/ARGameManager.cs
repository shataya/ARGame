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
    public GameObject monsterUI;
    public Text counter;
    public GameObject player;

    public DateTime startTime;

    public bool started = false;

    // Use this for initialization
    void Start()
    {
        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
        nd.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if(started)
        {
            var timetostart = startTime.Subtract(DateTime.Now).TotalSeconds;
            counter.text = timetostart.ToString();

            if(timetostart<=0)
            {
                //STARTEN
                started = false;
                counter.gameObject.SetActive(false);
                var ml = player.GetComponent<MonsterLauncher>();
                ml.ActivateEnemies();

            }
        }
    }


    public void startMatch()
    {
        Debug.Log("Start Match");
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
        nm.logLevel = LogFilter.FilterLevel.Info;
        nm.networkPort = 7777;
        nm.StartHost();
        nm.OnStartGame = startGame;
        nm.OnMonsterDataReceived = saveMonsterData;

        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
        nd.StartAsServer();

        startBlock.SetActive(false);
        lobbyBlock.SetActive(true);
    }


    public void findMatches()
    {
        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
        nd.StartAsClient();
    }


    public void joinMatch(int match)
    {
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
        nm.networkAddress = GetComponent<ARNetworkDiscoveryHandler>().DropdownMatches.options[match].text;
        nm.networkPort = 7777;
        nm.StartClient();
        startBlock.SetActive(false);
        lobbyBlock.SetActive(true);
        nm.OnStartGame = startGame;
        nm.OnMonsterDataReceived = saveMonsterData;
    }

    public void backToLobby()
    {
        lobbyBlock.SetActive(true);
        floorBlock.SetActive(false);
        monsterUI.SetActive(true);
    }

    public void placeSoldiers()
    {
        lobbyBlock.SetActive(false);
        floorBlock.SetActive(true);
        monsterUI.SetActive(false);
    }

    public void sendMonsterData()
    {
        var ml = player.GetComponent<MonsterLauncher>();
        var nm = this.gameObject.GetComponent<ARNetworkManager>();

        var mlArr = ml.MonsterDataList;
        nm.sendMonsterDataToServer(mlArr);

        startBlock.SetActive(false);
        lobbyBlock.SetActive(false);
        monsterUI.SetActive(false);
    }

    public void startGame(DateTime startTime)
    {
        counter.gameObject.SetActive(true);
        this.startTime = startTime;
        this.started = true;
    }

    public void saveMonsterData(MonsterDataMessage monsterDataMessage)
    {
        var ml = player.GetComponent<MonsterLauncher>();
        ml.SetEnemies(monsterDataMessage.clientId, monsterDataMessage.monsterData);
    }

    
}
