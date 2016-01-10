using UnityEngine;
using System.Collections;

public enum AttackMode : int
{
    Left = 0,
    Right
}

public class ARMonster : MonoBehaviour
{
    private static readonly Vector3 CENTER_OF_SCREEN = new Vector3 (0.5f, 0.5f, 0.0f);

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
    public float speed = 2000.0f;
    public float movingRadius = 10.0f;
    public float detectionRadius = 20.0f;
    public float attackInterval = 3.0f;

    public GameObject leftEnergyball;
    public GameObject rightEnergyball;

	void Awake ()
    {      
        Data = new MonsterData ();
        tempPos = transform.position;
        rigidbody = GetComponent<Rigidbody> ();
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
    }
	
	// Update is called once per frame
	void Update ()
    {
        SearchForPlayer ();
        
        if(canSeePlayer)
        {
            if(Time.time > nextAttack)
            {
                nextAttack = Time.time + attackInterval;
                Attack ();
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
                newSlidePos = tempPos + Random.insideUnitSphere * movingRadius;
                if (newSlidePos.y < tempPos.y)
                {
                    newSlidePos.y = tempPos.y;
                }
                //Debug.Log ("generated: " + newSlidePos);                    
            }

            var newTarget = (newSlidePos - rigidbody.position).normalized * speed;
            rigidbody.AddForce (newTarget);

            if (Vector3.Distance (rigidbody.position, newSlidePos) < positionReachThreshold)
            {
                generateNewPos = true;
                //Debug.Log ("reached: " + transform.position);
            }
        }
    }

    void LateUpdate()
    {
        transform.LookAt (player.transform);        
    }

    void OnCollisionStay()
    {
        //Debug.Log ("Collision detected");
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

        switch(attackMode)
        {
            case AttackMode.Left:
            ball = Instantiate (leftEnergyball, leftEnergyball.transform.position, leftEnergyball.transform.rotation) as GameObject;
            break;
            case AttackMode.Right:
            ball = Instantiate (rightEnergyball, rightEnergyball.transform.position, rightEnergyball.transform.rotation) as GameObject;
            break;
        }

        if(ball != null)
        {
            EnergyBall eb = ball.GetComponent<EnergyBall> ();
            eb.playerDir = player.transform.position;
            ball.SetActive (true);
        }
    }    
}
