using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class GOALPlanner 
{

    public Queue<GOAPAction> Plan(WorldState currentState, List<GOAPAction> actions, GOAPGoal goal)
    {

        Queue<GOAPAction> newPlan = new();

        //Calculate the new plan using the actions' preconditions and effects 

        return newPlan; 

    }


}
