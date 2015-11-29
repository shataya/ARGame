using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class CardboardMovement : MonoBehaviour
{
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
        horizontal = CrossPlatformInputManager.GetAxis (HORIZONTAL_NAME);
        vertical = CrossPlatformInputManager.GetAxis (VERTICAL_NAME);

        this.transform.Translate ((Vector3.forward * vertical + Vector3.right * horizontal) * Time.deltaTime * movementSpeed);
	}
}
