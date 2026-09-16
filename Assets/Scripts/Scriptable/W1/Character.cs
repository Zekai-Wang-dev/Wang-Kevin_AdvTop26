using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{

    public string name;
    public float health;
    public float speed;

    public float basePower;

    public List<Skill> skills; 

}

[System.Serializable]
public class Skill
{

    public string name;
    public List<Coin> coins; 

}

[System.Serializable]
public class Coin
{

    public float coinPower; 

}