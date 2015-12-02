using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GazeFuse : MonoBehaviour 
{
	public GameObject gazeGameObject;
	private Image image;

	void Start() 
	{
		image = GetComponent<Image>();
	}
	
	void Update() 
	{
		if (gazeGameObject == null || GazeInputModule.gazeGameObject == gazeGameObject) 
		{
			FuseAmountChanged(GazeInputModule.gazeFraction);
		}
	}

	void FuseAmountChanged(float fuseAmount)
	{
		if (image != null)
		{
			image.fillAmount = fuseAmount;
		}
	}

}
