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
        var buffer = new byte[16 * 1024];
        using (var ms = new MemoryStream (buffer, 0, buffer.Length))
        {
            bf.Serialize (ms, monsterData);
            writer.WriteBytesFull (ms.ToArray ());
        }            
    }

    public override void Deserialize(NetworkReader reader)
    {
        clientId = reader.ReadInt32 ();
        var payload = reader.ReadBytesAndSize ();
        var bf = new BinaryFormatter ();

        var buffer = new byte[16 * 1024];
        using (var ms = new MemoryStream (buffer, 0, buffer.Length))
        {
            monsterData = (MonsterData[])bf.Deserialize (ms);
        }
    }
}

