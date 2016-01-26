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


    /// <summary>
    /// Fügt einen Spieler zur Lobby hinzu
    /// </summary>
    /// <param name="client"></param>
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
        b.GetComponent<RectTransform>().transform.localScale = new Vector3(1, 1, 1);
        b.GetComponent<RectTransform>().transform.localRotation = Quaternion.identity;
        b.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 0, 0);
    }


    /// <summary>
    /// Sendet eine Nachricht zum Server, dass der Spieler bereit ist
    /// </summary>
    public void readyForMatch()
    {
        var nm = this.gameObject.GetComponent<ARNetworkManager>();
        nm.client.Send(MsgType.Ready, new ReadyMessage());
    }
}
