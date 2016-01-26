using UnityEngine;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking;

/// <summary>
/// Die Klasse dient als Daten-Modell für einen verbundenen Spieler,
/// gespeichert sind die eindeutige Connection-ID, ein Name (wird zurzeit nicht verwendet, ggf. für später als Erweiterung)
/// und ein Flag, ob der Spieler bereit ist, d.h. ob er alle seine Monster platziert hat.
/// </summary>
public class ARClient  {

    /// <summary>
    /// eindeutige Connection-ID, wird vom Server vergeben
    /// </summary>
    public int ClientConnectionId { get; set; }
    /// <summary>
    /// Name für einen Spieler, zurzeit nicht verwendet, sollte aber später vom Spieler frei wählbar sein (für die Anzeige)
    /// </summary>
    public string ClientName { get; set; }
    /// <summary>
    /// Flag, ob der Spieler bereit ist
    /// </summary>
    public bool Ready { get; set; }
}
