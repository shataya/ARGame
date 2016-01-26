using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

/// <summary>
/// Klasse zum Finden von Servern im lokalen Netzwerk
/// </summary>
public class ARNetworkDiscovery : NetworkDiscovery
{
    private NetworkClient client;
    private Dictionary<string, string> foundMatches;

    void Start()
    {
        foundMatches = new Dictionary<string, string>();

    } 

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
    
        if(!foundMatches.ContainsKey(fromAddress))
        {
            foundMatches.Add(fromAddress, data);
            this.gameObject.GetComponent<ARNetworkDiscoveryHandler>().onMatchFound(fromAddress);
        }
   
    }

    private void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("onconnected");
      
    }
}
