using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ARLobbyManager : MonoBehaviour
{
    private GameObject playerList;
    public GameObject buttonPrefab;

    private void Start()
    {
        playerList = GameObject.FindGameObjectWithTag("PlayerList");             
    }

    public void AddPlayer(ARClient client)
    {
        if (playerList == null)
        {
            playerList = GameObject.FindGameObjectWithTag("PlayerList");
        }
        GameObject b = Button.Instantiate<GameObject>(buttonPrefab);
        var text = b.GetComponentInChildren<Text>();
        text.text = client.ClientConnectionId.ToString();
        b.transform.parent = playerList.transform;
    }

    public void readyForMatch()
    {
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
        nm.client.Send(MsgType.Ready, new ReadyMessage());
    }
}
