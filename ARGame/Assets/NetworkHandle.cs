using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class NetworkHandle : NetworkManager {

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        conn.RegisterHandler(MsgType.Ready, OnReady);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

       
    }

    private void OnReady(NetworkMessage netMsg)
    {
        Debug.Log("HALLO");
       
        Debug.Log(netMsg.reader.ReadString());
    }
}
