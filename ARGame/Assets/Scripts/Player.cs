using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float currentHealth;
    public float completeHealth = 100.0f;
    public Vector3 spawnPoint = Vector3.zero;

	// Use this for initialization
	void Start () {
        currentHealth = completeHealth;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (currentHealth <= completeHealth)
        {
            // Regeneriere
            currentHealth = Mathf.Lerp (currentHealth, completeHealth, 0.00025f * Time.deltaTime);
            if (currentHealth > completeHealth)
            {
                currentHealth = completeHealth;
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
        Debug.Log ("gestorben");
    }
}
