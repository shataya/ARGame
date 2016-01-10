using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerStatusUpdateMessage : MessageBase {

    public int clientId;
    public int points;
    public int lifes;
}
