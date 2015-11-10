using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ARGameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
        nd.Initialize();
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void startMatch()
    {
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
        nm.logLevel = LogFilter.FilterLevel.Info;
        nm.networkPort = 7777;
        nm.StartHost();

        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
        nd.StartAsServer();
    }


    public void findMatches()
    {
        var nd = this.gameObject.GetComponent<ARNetworkDiscovery>();
        nd.StartAsClient();
    }

    
}
