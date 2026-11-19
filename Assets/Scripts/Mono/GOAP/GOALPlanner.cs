using JetBrains.Annotations;
using NodeCanvas.Tasks.Conditions;
using System.Collections.Generic;
using UnityEngine;

public class GOALPlanner 
{

    //A fuction to make the plan for the agent based on its current world state, avaiable actions, and the goal the planner is accomplishing 
    public Queue<GOAPAction> Plan(WorldState currentState, List<GOAPAction> actions, GOAPGoal goals)
    {

        Queue<GOAPAction> newPlan = new();

        if (CheckGoalAccomplished(goals, currentState)) { Debug.Log("Goal already accomplished");  return null; }

        foreach (var action in actions)
        {

            Dictionary<string, bool> Preconditions = action.Preconditions;

            bool conditionsMet = true;

            foreach (var condition in Preconditions)
            {

                if (currentState.CheckWorldState(condition.Key) != condition.Value) { conditionsMet = false; break; }

            }

            if (conditionsMet)
            {

                WorldState tempWorldState = currentState.Clone();

                foreach (var effect in action.Effects)
                {

                    tempWorldState.ChangeWorldState(effect.Key, effect.Value);

                }

                if (CheckGoalAccomplished(goals, tempWorldState))
                {

                    newPlan.Enqueue(action);
                    break; 

                }

            }

        }

        return newPlan; 

    }

    public bool CheckGoalAccomplished(GOAPGoal goals, WorldState worldState)
    {

        bool goalAchievable = true;

        foreach (var goal in goals.desiredStates)
        {

            if (worldState.CheckWorldState(goal.Key) != goal.Value) { goalAchievable = false; break; }

        }

        return goalAchievable;

    }

}
