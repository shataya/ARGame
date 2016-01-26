using UnityEngine;
using System.Collections;

/// <summary>
/// Klasse für die Energiekugel des ARMonsters
/// </summary>
public class EnergyBall : MonoBehaviour
{
    private Vector3 origin;
    private Vector3 target;
    private float time;

    [HideInInspector]
    public Vector3 playerDir;

    [HideInInspector]
    public float damage;

    public float speed = 0.025f;

    void Awake()
    {
        
    }

    void Start()
    {
        time = Time.time;
    }

    void Update()
    {        
        transform.position = Vector3.Lerp (transform.position, playerDir, Time.deltaTime * speed);

        if(Time.time > time + 5.0f)
        {
            Destroy (gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if((collision.gameObject.layer & 8) != 8)
        {
            Destroy (gameObject);
        }
    }
}
