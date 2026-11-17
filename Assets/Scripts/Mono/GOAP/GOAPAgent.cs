using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GOAPAgent : MonoBehaviour
{

    public WorldState currentState = new WorldState();

    public List<GOAPAction> actions;
    public List<GOAPGoal> goals;
    public GOALPlanner goalPlanner; 

}
