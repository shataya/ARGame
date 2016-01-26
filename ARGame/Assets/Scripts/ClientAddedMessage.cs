using UnityEngine;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking;

/// <summary>
/// Nachricht, wenn ein Spieler sich verbunden hat
/// </summary>
public class ClientAddedMessage : MessageBase
{
    public int ClientConnectionId;
    public string ClientName;	
}
