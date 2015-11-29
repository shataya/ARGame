using UnityEngine;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking;

public class ClientAddedMessage : MessageBase{

    public int ClientConnectionId;
    public string ClientName;
	
}
