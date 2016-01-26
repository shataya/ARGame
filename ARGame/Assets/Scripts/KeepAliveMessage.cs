using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Nachricht zum Senden einer Keepalive-Message, um die Verbindung zum Server aufrechtzuerhalten
/// </summary>
public class KeepAliveMessage : MessageBase
{

    public int clientId;

}
