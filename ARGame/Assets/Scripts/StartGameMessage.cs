using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

/// <summary>
/// Nachricht zum Starten eines Spiels, nachdem alle Spieler bereit sind
/// </summary>
public class StartGameMessage : MessageBase
{
    public long startTime;
}

