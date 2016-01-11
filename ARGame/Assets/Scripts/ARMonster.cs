using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public enum AttackMode : int
{
    Left = 0,
    Right
}

public class ARMonster : MonoBehaviour
{
    private GameObject player;
    private new Rigidbody rigidbody;
    private Animator animator;

    private bool canSeePlayer;
    private bool generateNewPos = true;

    // kommt weg, eig stehen daten in monsterdata
    private Vector3 tempPos;
    private Vector3 newSlidePos;
    private int attackHash = Animator.StringToHash ("Attack");
    private int blockHash = Animator.StringToHash ("Block");
    private float nextAttack = 0.0f;

    public int MonsterId { get; set; }
    public int ClientId { get; set; }
    public MonsterData Data { get; set; }

    public float positionReachThreshold = 0.5f;
    public float speed = 400.0f;
    public float movingRadius = 10.0f;
    public float detectionRadius = 20.0f;
    public float attackInterval = 2.0f;
    public bool canInteract = true;
    public float health = 10000.0f;

    public GameObject leftEnergyball;
    public GameObject rightEnergyball;
    public GameObject hitInfoText;

    public Action<int> OnDie;

	void Awake ()
    {      
        Data = new MonsterData ();
        tempPos = transform.position;        
        animator = GetComponent<Animator> ();               
    }

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindWithTag ("Player");
        if (!player)
        {
            Debug.LogError ("Player nicht gefunden");
        }
        
        if(canInteract)
        {
            rigidbody = gameObject.AddComponent<Rigidbody> ();
        }            
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(canInteract)
        {
            SearchForPlayer ();

            if (canSeePlayer)
            {
                if (Time.time > nextAttack)
                {
                    nextAttack = Time.time + attackInterval;
                    Attack ();
                }
            }
        }              
    }

    void FixedUpdate()
    {
        // Falls gefunden, bewege dich!
        if (canSeePlayer)
        {
            if (generateNewPos)
            {
                generateNewPos = false;
                newSlidePos = tempPos + UnityEngine.Random.insideUnitSphere * movingRadius;
                if (newSlidePos.y < tempPos.y)
                {
                    newSlidePos.y = tempPos.y;
                }                    
            }

            var newTarget = (newSlidePos - rigidbody.position).normalized * speed;
            rigidbody.AddForce (newTarget);

            if (Vector3.Distance (rigidbody.position, newSlidePos) < positionReachThreshold)
            {
                generateNewPos = true;
            }
        }
    }

    void LateUpdate()
    {
        if(canInteract)
        {
            transform.LookAt (player.transform);
        }             
    }

    void OnCollisionStay()
    {
        generateNewPos = true;
    }

    void SearchForPlayer()
    {
        // Suche nach Gegner
        Ray ray = new Ray (transform.position, player.transform.position - transform.position);
        RaycastHit hitInfo;

        Debug.DrawRay (ray.origin, ray.direction, Color.red, 1, false);
        if (Physics.Raycast (ray, out hitInfo, detectionRadius))
        {
            if (hitInfo.collider.tag.Equals ("Player"))
            {
                canSeePlayer = true;
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else
        {
            canSeePlayer = false;
        }
    }

    void Attack()
    {
        animator.SetBool (attackHash, true);
    }

    void Shoot(int mode)
    {
        GameObject ball = null;
        AttackMode attackMode = (AttackMode)mode;

        switch (attackMode)
        {
            case AttackMode.Left:
            ball = Instantiate (leftEnergyball, leftEnergyball.transform.position, leftEnergyball.transform.rotation) as GameObject;
            break;
            case AttackMode.Right:
            ball = Instantiate (rightEnergyball, rightEnergyball.transform.position, rightEnergyball.transform.rotation) as GameObject;
            break;
        }

        if (ball != null)
        {
            EnergyBall eb = ball.GetComponent<EnergyBall> ();
            eb.playerDir = player.transform.position;
            eb.damage = Data.attackValue / 2.0f;
            ball.SetActive (true);
        }
    }

    IEnumerator Die()
    {
        canInteract = false;
        yield return new WaitForSeconds (1.0f);

        Destroy (gameObject);
    
        OnDie(ClientId);
    }

    IEnumerator AnimateHitInfo(GameObject hitInfo)
    {
        Material material = hitInfo.GetComponent<MeshRenderer> ().material;
        Transform transform = hitInfo.transform;
        Vector3 target = transform.position + Vector3.up * 2.0f;
        float animTime = 4.0f;

        while(material.color.a > 0.1f)
        {
            material.color = Color.Lerp (material.color, Color.clear, 0.5f * animTime * Time.deltaTime);
            transform.position = Vector3.Lerp (transform.position, target, animTime * Time.deltaTime);        

            yield return null;
        }        
        Destroy (hitInfo);
        yield break;
    }

    public void TakeHit(Vector3 point, HitMode mode)
    {
        if(canInteract)
        {            
            float damage = UnityEngine.Random.Range(100f, 200f);
            float reducedInnerFactor = 0.75f;
            float reducedOuterFactor = 0.25f;
            switch (mode)
            {                
                case HitMode.LeftInnerWing:
                case HitMode.RightInnerWing:
                damage *= reducedInnerFactor;
                break;
                case HitMode.LeftOutterWing:  
                case HitMode.RightOuterWing:
                damage *= reducedOuterFactor;
                break;
            }
            health -= damage;
            if (health <= 0.0f)
            {
                StartCoroutine (Die ());
            }
            else
            {
                GameObject hitInfo = Instantiate (hitInfoText, point, Quaternion.identity) as GameObject;
                TextMesh mesh = hitInfo.GetComponent<TextMesh> ();
                mesh.text = Mathf.Round ((float)damage).ToString ();
                StartCoroutine (AnimateHitInfo (hitInfo));
            }     
        }        
    }
}
