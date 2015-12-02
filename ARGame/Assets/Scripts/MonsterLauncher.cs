using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MonsterLauncher : MonoBehaviour
{
    public GameObject monsterPrefab;

    private Dictionary<int, GameObject> placedMonsters;

    void Awake()
    {
        placedMonsters = new Dictionary<int, GameObject> ();
    }

	// Use this for initialization
	void Start ()
    {
	    if(!monsterPrefab)
        {
            Debug.LogError ("Kein Monster-Prefab zugewiesen.");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void PlaceMonster(int id)
    {
        if (!placedMonsters.ContainsKey (id) && 
            placedMonsters.Count (kv => GameUtil.IsInRadius (transform.position, 10.0f, kv.Value.transform.position)) == 0)
        {
            GameObject monsterInstance = Instantiate (monsterPrefab, transform.position, transform.rotation) as GameObject;
            Transform monsterTransform = monsterInstance.transform;

            ARMonster monster = monsterInstance.GetComponent<ARMonster> ();
            monster.MonsterId = id;
            monster.name = string.Format ("Monster Id: {0}", id);

            MonsterData data = monster.Data;
            data.id = id;
            data.position = monsterTransform.position;
            data.rotation = monsterTransform.rotation;

            placedMonsters.Add (id, monsterInstance);
            // TODO: Button ausgrauen
        }        
        else
        {
            Debug.LogError ("Monster konnte nicht platziert werden. Entweder bereits gesetzt oder der aktuelle Punkt ist zu nah an einem anderen.");
            // TODO: Irgendwas ausgeben
        }
    }
}
