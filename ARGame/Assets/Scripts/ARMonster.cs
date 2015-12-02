using UnityEngine;
using System.Collections;

public class ARMonster : MonoBehaviour
{
    public int MonsterId { get; set; }
    public int ClientId { get; set; }

    public MonsterData Data { get; set; }
	
	void Awake ()
    {
        Data = new MonsterData ();
	}

    // Use this for initialization
    void Start()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}
}
