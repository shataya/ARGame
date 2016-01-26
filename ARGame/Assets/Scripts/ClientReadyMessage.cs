using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Nachricht, wenn ein Spieler bereit ist
/// </summary>
public class ClientReadyMessage : MessageBase {

    public int ClientConnectionId;
}
