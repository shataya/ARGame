using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Text;
using System;

public class MonsterLauncher : MonoBehaviour
{
    public GameObject monsterPrefab;

    public List<Text> attackMonsters;
    public List<Text> defMonsters;
    public List<Image> monsterImages;
    public List<Button> skillsButtons;

    private Dictionary<int, GameObject> placedMonsters;
    private Dictionary<int, List<GameObject>> enemyMonsters;
    private List<GameObject> enemyGroups;

    public int attackValueAmount = 40;
    public int defenseValueAmount = 40;

    public int attackValueUsed = 0;
    public int defenseValueUsed = 0;

    public Text attackValueStatus;
    public Text defenseValueStatus;

    private int skillsActive = -1;

    private List<MonsterData> monsterDatas;

    private float monsterDetectionRadius;

    public MonsterData[] MonsterDataList
    {
        get
        {
            return placedMonsters.Select(m => m.Value.GetComponent<ARMonster>().Data).ToArray();
        }
    }

    void Awake()
    {
        placedMonsters = new Dictionary<int, GameObject> ();
        enemyMonsters = new Dictionary<int, List<GameObject>> ();
        enemyGroups = new List<GameObject>();
        monsterDetectionRadius = monsterPrefab.GetComponent<ARMonster> ().detectionRadius;
    }

	// Use this for initialization
	void Start ()
    {
	    if(!monsterPrefab)
        {
            Debug.LogError ("Kein Monster-Prefab zugewiesen.");
        }
        monsterDatas = new List<MonsterData>();
        for(int i = 0; i < 4; i++)
        {
            monsterDatas.Add(new MonsterData());
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void PlaceMonster(int id)
    {
        if(placedMonsters.ContainsKey(id) && placedMonsters.Count(kv => GameUtil.IsInRadius(transform.position, monsterDetectionRadius, kv.Value.transform.position)) == 0)
        {
            //remove first
            Destroy(placedMonsters[id]);
            placedMonsters.Remove(id);
            
            //...??
        }
        if (!placedMonsters.ContainsKey (id) && 
            placedMonsters.Count (kv => GameUtil.IsInRadius (transform.position, 10.0f, kv.Value.transform.position)) == 0)
        {
            monsterImages[id].color = new Color(1,1,1,0.5f);

            GameObject monsterInstance = Instantiate (monsterPrefab, transform.position, transform.rotation) as GameObject;
            monsterInstance.name = string.Format ("Monster Id: {0}", id);
            Transform monsterTransform = monsterInstance.transform;

            ARMonster monster = monsterInstance.GetComponent<ARMonster> ();
            monster.Data = monsterDatas[id];
            monster.MonsterId = id;

            MonsterData data = monster.Data;
            data.id = id;
            data.posX = monsterTransform.position.x;
            data.posY = monsterTransform.position.y;
            data.posZ = monsterTransform.position.z;
            data.rotX = monsterTransform.rotation.x;
            data.rotY = monsterTransform.rotation.y;
            data.rotZ = monsterTransform.rotation.z;
            data.rotW = monsterTransform.rotation.w;
            attackMonsters[id].text = data.attackValue.ToString();
            defMonsters[id].text = data.defenseValue.ToString();
            placedMonsters.Add (id, monsterInstance);
    
          
            attackMonsters[id].text = monster.Data.attackValue.ToString();
            defMonsters[id].text = monster.Data.defenseValue.ToString();
            updateStatus();

        }        
        else
        {
            Debug.LogError ("Monster konnte nicht platziert werden. Entweder bereits gesetzt oder der aktuelle Punkt ist zu nah an einem anderen.");
            // TODO: Irgendwas ausgeben
        }
    }

    public void activateSkills(int id)
    {
        if(skillsActive>=0)
        {
            skillsButtons[skillsActive].interactable = true;
        }
     
        skillsActive = id;
        skillsButtons[id].interactable = false;
        
    }

    private void updateStatus()
    {
        
        attackValueStatus.text = String.Format("{0}/{1}", attackValueUsed, attackValueAmount);
        defenseValueStatus.text = String.Format("{0}/{1}", defenseValueUsed, defenseValueAmount);
    }


    public void AddAttackValue(int value)
    {
        if(attackValueUsed+value <= attackValueAmount)
        {
            MonsterData monsterData = monsterDatas[skillsActive];
            if(monsterData.attackValue +value >=0)
            {
                monsterData.attackValue += value;
                attackMonsters[skillsActive].text = monsterData.attackValue.ToString();
                attackValueUsed += value;
                updateStatus();
            }
           
        }
       

    }

    public void AddDefenseValue(int value)
    {
        if (defenseValueUsed + value <= defenseValueAmount)
        {
            MonsterData monsterData = monsterDatas[skillsActive];
            if (monsterData.defenseValue+value >=0)
            {
                monsterData.defenseValue += value;
                defMonsters[skillsActive].text = monsterData.defenseValue.ToString();
                defenseValueUsed += value;
                updateStatus();
            }
           
        }
    }

    public void SetEnemies(int id, MonsterData[] data)
    {
        if(!enemyMonsters.ContainsKey(id))
        {
            enemyMonsters.Add (id, new List<GameObject> ());
            GameObject enemy = new GameObject (string.Format ("Enemy Id: {0}", id));
            enemy.SetActive (false);
            enemyGroups.Add(enemy);

            foreach(var enemyMonster in data)
            {
                GameObject monsterInstance = Instantiate (monsterPrefab, new Vector3(enemyMonster.posX, enemyMonster.posY, enemyMonster.posZ), new Quaternion(enemyMonster.rotX, enemyMonster.rotY, enemyMonster.rotZ, enemyMonster.rotW)) as GameObject;
                monsterInstance.name = string.Format ("Monster Id: {0}", enemyMonster.id);
                monsterInstance.transform.SetParent(enemy.transform);

                ARMonster monster = monsterInstance.GetComponent<ARMonster> ();
                monster.MonsterId = enemyMonster.id;
                monster.Data = enemyMonster;

                enemyMonsters[id].Add (monsterInstance);
            }
        }
    }

    public void ActivateEnemies()
    {
        foreach(var ownMonster in placedMonsters)
        {
            ownMonster.Value.SetActive (false);
        }

        foreach (var enemy in enemyGroups)
        {
            enemy.SetActive(true);
        }
    }
}
