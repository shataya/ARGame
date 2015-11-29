using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ARGameManager : MonoBehaviour
{
    public GameObject startBlock;
    public GameObject lobbyBlock;

    // Use this for initialization
    void Start()
    {
        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
        nd.Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void startMatch()
    {
        Debug.Log("Start Match");
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
        nm.logLevel = LogFilter.FilterLevel.Info;
        nm.networkPort = 7777;
        nm.StartHost();

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
    }
}
