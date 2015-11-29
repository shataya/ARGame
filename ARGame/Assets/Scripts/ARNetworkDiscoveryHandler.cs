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
        var option = new Dropdown.OptionData();
        option.text = "Kein Match ausgewählt";
        DropdownMatches.options.Add(option);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
