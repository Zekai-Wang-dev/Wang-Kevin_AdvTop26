using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//The agent's properties that include its avaiable actions, its goals, as well as the planenr its current world state. 
public class GOAPAgent : MonoBehaviour
{

    public WorldState currentState = new WorldState();
    public GOALPlanner goalPlanner = new GOALPlanner();

    public List<GOAPAction> actions;
    public GOAPGoal goals;

    private Queue<GOAPAction> plannedActions; 

    public void PlanActions()
    {

        plannedActions = goalPlanner.Plan(currentState, actions, goals);

    }

    public void ExecuteNextAction()
    {

        GOAPAction action = plannedActions.Dequeue();

        foreach(var effect in action.Effects)
        {

            currentState.ChangeWorldState(effect.Key, effect.Value); 

        }

        action.Execute(this); 

    }

}
