using System;
using UnityEngine;

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
 

    public int attackValue = 10;
    public int defenseValue = 10;    

	public MonsterData()
    {
        
    }
}
