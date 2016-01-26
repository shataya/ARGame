using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Attack Modus: Linker oder Rechter Flügel
/// </summary>
public enum AttackMode : int
{
    Left = 0,
    Right
}

/// <summary>
/// Basisklasse für das Monsterverhalten
/// </summary>
public class ARMonster : MonoBehaviour
{
    private GameObject player;
    private new Rigidbody rigidbody;
    private Animator animator;
    private AudioSource hitSound;

    private bool canSeePlayer;
    private bool generateNewPos = true;

    private Vector3 tempPos;
    private Vector3 newSlidePos;
    private int attackHash = Animator.StringToHash ("Attack");
    private int blockHash = Animator.StringToHash ("Block");
    private float nextAttack = 0.0f;
    private float completeHealth;
    private int attackCounter = 0;
    private bool isBlocking;

    public int MonsterId { get; set; }
    public int ClientId { get; set; }
    public MonsterData Data { get; set; }

    public float positionReachThreshold = 0.5f;
    public float speed = 400.0f;
    public float movingRadius = 10.0f;
    public float detectionRadius = 20.0f;
    public float attackInterval = 0.5f;
    public bool canInteract = true;

    public float basicHealth = 10000.0f;
    public float currentHealth;
    public float baseAttackPower = 5.0f;
    public int blockAfterXAttacks = 3;

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

        completeHealth = basicHealth + Data.defenseValue * 100.0f;
        currentHealth = completeHealth;

        var audioComp = GetComponent<AudioSource>();
        if (!canInteract)
        {
            audioComp.Play();
        }
        else if(canInteract)
        {
            hitSound = audioComp;
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
                    attackCounter += 1;

                    if(attackCounter != blockAfterXAttacks + 1)
                    {
                        isBlocking = false;
                        Attack ();
                    }
                    else
                    {
                        Block ();
                        isBlocking = true;
                        attackCounter = 0;
                    }                    
                }
            }
            else
            {
                if (currentHealth <= completeHealth)
                {
                    // Regeneriere
                    currentHealth = Mathf.Lerp (currentHealth, completeHealth, 0.25f * Time.deltaTime);
                    if (currentHealth > completeHealth)
                    {
                        currentHealth = completeHealth;
                    }
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

    /// <summary>
    /// Gegnersuche
    /// </summary>
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

    /// <summary>
    /// Angriffsanimation starten
    /// </summary>
    void Attack()
    {
        animator.SetBool (attackHash, true);
    }

    /// <summary>
    /// Blockanimation starten
    /// </summary>
    void Block()
    {
        animator.SetBool (blockHash, true);
    }

    /// <summary>
    /// Schießen eines Energyballs
    /// </summary>
    /// <param name="mode">Gibt an, in welchem Modus die Attacke ausgeführt wird</param>
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
            eb.playerDir = UnityEngine.Random.Range (0.0f, 1.0f) < 0.5f ? player.transform.position : transform.forward * 20.0f;
            eb.damage = baseAttackPower + Data.attackValue / 2.0f;
            ball.SetActive (true);
        }
    }

    /// <summary>
    /// Starte Sterbe-Routine
    /// </summary>
    /// <returns></returns>
    IEnumerator Die()
    {
        canInteract = false;
        yield return new WaitForSeconds (1.0f);

        Destroy (gameObject);
    
        OnDie(ClientId);
    }

    /// <summary>
    /// Coroutine, die die Trefferzahl anzeigt (mit Animation)
    /// </summary>
    /// <param name="hitInfo">Beinhaltet Informationen über den Treffer</param>
    /// <returns></returns>
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

    /// <summary>
    /// Trefferregistrierung und -folgen (Energiereduktion, evtl. Tod)
    /// </summary>
    /// <param name="point">Trefferstelle</param>
    /// <param name="mode">Treffermodus</param>
    public void TakeHit(Vector3 point, HitMode mode)
    {
        if(canInteract && canSeePlayer)
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

            if(!isBlocking)
            {
                currentHealth -= damage;
            }
            
            if (currentHealth <= 0.0f)
            {
                StartCoroutine (Die ());
            }
            else
            {
                hitSound.Play();
                Vector3 rot = transform.rotation.eulerAngles;
                rot.y += 180.0f;
                GameObject hitInfo = Instantiate (hitInfoText, point, Quaternion.Euler(rot)) as GameObject;
                TextMesh mesh = hitInfo.GetComponent<TextMesh> ();
                mesh.text = isBlocking ? "BLOCK" : Mathf.Round ((float)damage).ToString ();
                StartCoroutine (AnimateHitInfo (hitInfo));
            }
        }        
    }
}
