using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

public class MonsterDataMessage : MessageBase
{
    public List<MonsterData> monsterData;
    public int clientId;
}

