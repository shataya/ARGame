using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class Player : MonoBehaviour
{
    public Transform headChild;
    public float movementSpeed = 3.0f;

    private const string HORIZONTAL_NAME = "Horizontal";
    private const string VERTICAL_NAME = "Vertical";

    private float vertical;
    private float horizontal; 

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        vertical = Input.GetAxis (VERTICAL_NAME);
        this.transform.Translate (headChild.forward * vertical * Time.deltaTime * movementSpeed);                
	}
}
