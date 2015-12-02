using UnityEngine;
using System.Collections;

public class ARMonster : MonoBehaviour
{
    private Animator animator;

    public int ClientId { get; set; }

	// Use this for initialization
	void Start ()
    {
        this.animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}
}
