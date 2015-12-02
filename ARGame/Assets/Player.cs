using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    private List<ARMonster> monster;

    public GameObject monsterPrefab;

    void Awake()
    {
        this.monster = new List<ARMonster> ();
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    { 
	    if(Input.GetKeyDown(KeyCode.F))
        {
            
        }
	}
}
