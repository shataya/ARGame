using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ARNetworkDiscovery : NetworkDiscovery
{
    private bool isConnected;
    private NetworkClient client;

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        if (!isConnected)
        {
            isConnected = true;
            Debug.LogFormat("Recieved from {0}", fromAddress);

            NetworkManager nm = this.gameObject.AddComponent<NetworkManager>();
            nm.logLevel = LogFilter.FilterLevel.Debug;
            nm.networkAddress = fromAddress;
            nm.networkPort = 7777;

            client = nm.StartClient();
            client.RegisterHandler(MsgType.Connect, OnConnected);
        }
    }

    private void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("onconnected");
        if(!client.Send(MsgType.Ready, new StringMessage("Reaaaady :D")))
        {
            Debug.LogError("Send misslungen");
        }
    }
}
