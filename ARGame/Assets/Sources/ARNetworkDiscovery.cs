using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ARNetworkDiscovery : NetworkDiscovery
{
    //private bool isConnected;
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
        


    /*    if (!isConnected)
        {
            isConnected = true;
            Debug.LogFormat("Recieved from {0}", fromAddress);
            
            NetworkManager nm = this.GetComponent<NetworkManager>();
            nm.logLevel = LogFilter.FilterLevel.Info;
            nm.networkAddress = fromAddress;
            nm.networkPort = 7777;

            client = nm.StartClient();
            client.RegisterHandler(MsgType.Connect, OnConnected);
        }*/
    }

    private void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("onconnected");
      /*  var addPlayerMessage = new AddPlayerMessage();
       
        if(!client.Send(MsgType.AddPlayer, new AddPlayerMessage()))
        {
            Debug.LogError("Send misslungen");
        }*/
    }
}
