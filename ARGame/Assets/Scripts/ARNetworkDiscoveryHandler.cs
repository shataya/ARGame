using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Klasse zum Handlen des Dropdown-Menüs für die gefundene Matches-Anzeige
/// </summary>
public class ARNetworkDiscoveryHandler : MonoBehaviour {

    public Dropdown DropdownMatches;


    /// <summary>
    /// Fügt einen neuen Eintrag zum Dropdown-Menü hinzu
    /// </summary>
    /// <param name="address">IP-Adresse des Matches</param>
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
