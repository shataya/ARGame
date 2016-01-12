using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public float currentHealth;
    public float completeHealth = 100.0f;
    public float lowHealth = 20.0f;
    public Vector3 spawnPoint = Vector3.zero;

    public UnityEvent onPlayerDie;

    private AudioSource lowHealthSound;

	// Use this for initialization
	void Start () {
        currentHealth = completeHealth;
        lowHealthSound = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (currentHealth <= completeHealth)
        {
            // Regeneriere
            currentHealth = Mathf.Lerp (currentHealth, completeHealth, 0.01f * Time.deltaTime);
            if (currentHealth > completeHealth)
            {
                currentHealth = completeHealth;
            }

            else if(currentHealth <= lowHealth && !lowHealthSound.isPlaying) 
            {
                lowHealthSound.Play();
            } else if(currentHealth > lowHealth && lowHealthSound.isPlaying)
            {
                lowHealthSound.Stop();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Equals("EnergyBall"))
        {           
            EnergyBall eb = collision.gameObject.GetComponent<EnergyBall> ();
            if(eb != null)
            {
                Debug.Log ("hit by energyball");
                currentHealth -= eb.damage;
                if (currentHealth <= 0.0f)
                    Die ();
            }
        }
    }

    void Die()
    {
        transform.position = spawnPoint;
        currentHealth = 0.75f * completeHealth;
        Debug.Log ("gestorben");
    }
}
