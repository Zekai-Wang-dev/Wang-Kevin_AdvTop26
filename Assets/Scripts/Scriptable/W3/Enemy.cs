using UnityEngine;

public abstract class Enemy : ScriptableObject
{

    public float health;
    public float height;

    public abstract void Attack();

}

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy/GuyOnHorse")]
public class GuyOnHorse : Enemy
{
    public Color horseColor;

    public float joustLength; 
    public override void Attack()
    {

        Debug.Log("Meow"); 

    }

}

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy/BowGuy")]
public class BowGuy : Enemy
{

    public void Reload()
    {


    }

    public override void Attack()
    {

        //Shoot bow

    }

}