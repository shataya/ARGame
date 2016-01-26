using System;
using UnityEngine;

/// <summary>
/// Monsterdaten, die über das Netzwerk serialisiert werden
/// </summary>
[Serializable]
public class MonsterData
{
    public int id;

    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;
    public float rotW; 

    public int attackValue = 0;
    public int defenseValue = 0;    

	public MonsterData()
    {
        
    }
}
