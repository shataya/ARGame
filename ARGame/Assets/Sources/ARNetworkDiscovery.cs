using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ARNetworkDiscovery : NetworkDiscovery
{

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    { 
	
	}

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        Debug.LogFormat("Recieved from {0}", fromAddress);

        ConnectionConfig config = new ConnectionConfig();
        int reiliableChannelId = config.AddChannel(QosType.Reliable);

        HostTopology topology = new HostTopology(config, 10);
        this.hostId = NetworkTransport.AddHost(topology, 50000);

        string msg = "Hallo von Klaus.";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);

        byte error;
        int connectionId = NetworkTransport.Connect(this.hostId, fromAddress, 50000, 0, out error);
        NetworkTransport.Send(hostId, connectionId, reiliableChannelId, buffer, buffer.Length, out error);
    }
}
