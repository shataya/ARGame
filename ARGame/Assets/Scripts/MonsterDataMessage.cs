using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Netzwerknachricht, die Monsterdaten beinhaltet
/// </summary>
public class MonsterDataMessage : MessageBase
{
    public MonsterData[] monsterData;
    public int clientId;

    /// <summary>
    /// Serialisierung der Monsterdaten
    /// </summary>
    /// <param name="writer"></param>
    public override void Serialize(NetworkWriter writer)
    {
        writer.Write (clientId);

        var bf = new BinaryFormatter ();
        using (var ms = new MemoryStream ())
        {
            bf.Serialize (ms, monsterData);
            var binArr = ms.ToArray ();
            writer.WriteBytesFull (binArr);
        }            
    }

    /// <summary>
    /// Deserialisierung der Monsterdaten
    /// </summary>
    /// <param name="reader"></param>
    public override void Deserialize(NetworkReader reader)
    {
        clientId = reader.ReadInt32 ();
        var payload = reader.ReadBytesAndSize ();
        var bf = new BinaryFormatter ();

        using (var ms = new MemoryStream (payload,0,payload.Length))
        {
            monsterData = (MonsterData[])bf.Deserialize (ms);
        }
    }
}

