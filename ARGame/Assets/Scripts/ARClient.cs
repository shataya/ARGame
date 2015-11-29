using UnityEngine;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking;

public class ARClient  {

    public int ClientConnectionId { get; set; }
    public string ClientName { get; set; }
    public bool Ready { get; set; }
}
