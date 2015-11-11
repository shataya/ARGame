using UnityEngine;
using System.Collections;

public class ARMonster : MonoBehaviour {
    private Animator animator;

	// Use this for initialization
	void Start ()
    {
        this.animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        bool attack = Input.GetButton("Fire1");
        bool block = Input.GetButton("Fire2");

        if (attack)
        {
            animator.SetTrigger("Attack");
        }
        else if (block)
        {
            animator.SetTrigger("Block");
        }
	}
}
