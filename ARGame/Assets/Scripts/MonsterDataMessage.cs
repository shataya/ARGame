using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MonsterDataMessage : MessageBase
{
    public MonsterData[] monsterData;
    public int clientId;

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write (clientId);

        var bf = new BinaryFormatter ();
        using (var ms = new MemoryStream ())
        {
            Debug.Log ("schreibe...");
            bf.Serialize (ms, monsterData);
            var binArr = ms.ToArray ();
            Debug.Log ("msarr ser: " + binArr.Length);
            writer.WriteBytesFull (binArr);
        }            
    }

    public override void Deserialize(NetworkReader reader)
    {
        clientId = reader.ReadInt32 ();
        var payload = reader.ReadBytesAndSize ();
        Debug.Log ("payoad size: " + (payload == null ? "" : payload.Length.ToString()));
        var bf = new BinaryFormatter ();

        using (var ms = new MemoryStream (payload))
        {
            Debug.Log ("lese...");
            monsterData = (MonsterData[])bf.Deserialize (ms);
            Debug.Log ("msarr des: " + monsterData.Length);
        }
    }
}

