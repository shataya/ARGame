using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Nachricht zum Updaten der Punkte und Leben eines Spielers
/// </summary>
public class PlayerStatusUpdateMessage : MessageBase {

    public int clientId;
    public int points;
    public int lifes;
}
