using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MonsterLauncher : MonoBehaviour
{
    public GameObject monsterPrefab;

    private Dictionary<int, GameObject> placedMonsters;
    private Dictionary<int, List<GameObject>> enemyMonsters;

    public List<MonsterData> MonsterDataList
    {
        get
        {
            return placedMonsters.Select(m => m.Value.GetComponent<ARMonster>().Data).ToList();
        }
    }

    void Awake()
    {
        placedMonsters = new Dictionary<int, GameObject> ();
        enemyMonsters = new Dictionary<int, List<GameObject>> ();
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
            monsterInstance.name = string.Format ("Monster Id: {0}", id);
            Transform monsterTransform = monsterInstance.transform;

            ARMonster monster = monsterInstance.GetComponent<ARMonster> ();
            monster.MonsterId = id;

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

    public void SetEnemies(int id, List<MonsterData> data)
    {
        if(!enemyMonsters.ContainsKey(id))
        {
            enemyMonsters.Add (id, new List<GameObject> ());
            GameObject enemy = new GameObject (string.Format ("Enemy Id: {0}", id));
            enemy.SetActive (false);

            foreach(var enemyMonster in data)
            {
                GameObject monsterInstance = Instantiate (monsterPrefab, enemyMonster.position, enemyMonster.rotation) as GameObject;
                monsterInstance.name = string.Format ("Monster Id: {0}", enemyMonster.id);
                monsterInstance.transform.parent = enemy.transform;

                ARMonster monster = monsterInstance.GetComponent<ARMonster> ();
                monster.MonsterId = enemyMonster.id;
                monster.Data = enemyMonster;

                enemyMonsters[id].Add (monsterInstance);
            }
        }
    }
}
