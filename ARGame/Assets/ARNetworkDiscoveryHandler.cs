using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ARNetworkDiscoveryHandler : MonoBehaviour {

    public Dropdown DropdownMatches;


    public void onMatchFound(string address)
    {
        var option = new Dropdown.OptionData();
        option.text = address;
        DropdownMatches.options.Add(option);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
