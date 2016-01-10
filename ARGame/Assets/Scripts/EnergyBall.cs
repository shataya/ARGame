using UnityEngine;
using System.Collections;

public class EnergyBall : MonoBehaviour
{
    private Vector3 origin;
    private Vector3 target;
    private float time;

    [HideInInspector]
    public Vector3 playerDir;



    public float speed = 0.1f;

    void Awake()
    {
        
    }

    void Start()
    {
        origin = transform.position - Vector3.up;
        target = (playerDir - origin).normalized * 20.0f;
        time = Time.time;
    }

    void Update()
    {
        transform.position = Vector3.Lerp (transform.position, target, Time.deltaTime * speed);

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
