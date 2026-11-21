using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GOALPlanner 
{

    private float totalCost = 0; 

    //A fuction to make the plan for the agent based on its current world state, avaiable actions, and the goal the planner is accomplishing 
    public Queue<GOAPAction> Plan(WorldState currentState, List<GOAPAction> actions, GOAPGoal goal, int maxNodeExpansions)
    {
        totalCost = float.PositiveInfinity;

        Debug.Log("Planning actions"); 

        if (CheckGoalAccomplished(goal, currentState)) { Debug.Log("Goal already accomplished"); totalCost = 0; return new(); }

        GOAPNode initNode = new(currentState, null, null, 0);

        List<GOAPNode> nodeList = new();

        List<(WorldState, float)> savedWorldStates = new(); 

        nodeList.Add(initNode);

        savedWorldStates.Add((currentState, 0));

        for (int i = 0; i < maxNodeExpansions; i++)
        {

            GOAPNode prevNode = FindCheapestNodeInList(nodeList);

            if (prevNode == null || prevNode.totalCost < 0) return null; 

            nodeList.Remove(prevNode);

            if (IsOudatedNode(savedWorldStates, prevNode)) { i--; Debug.Log("Removing higher cost nodes");  continue; }

            if (CheckGoalAccomplished(goal, prevNode.currenState))
            {

                Queue<GOAPAction> plannedActions = BacktrackActionsFromNodeList(prevNode);

                if (plannedActions != null)
                {
                    SetTotalCostOfPlan(prevNode);
                    return plannedActions;
                }
                else return null; 

            }

            foreach (var action in actions)
            {

                if (action.Cost < 0) return null;

                Dictionary<string, bool> Preconditions = action.Preconditions;

                bool conditionsMet = true;

                foreach (var condition in Preconditions)
                {

                    if (!prevNode.currenState.CheckWorldState(condition.Key, condition.Value)) { conditionsMet = false; break; }

                }

                if (conditionsMet)
                {

                    WorldState tempWorldState = prevNode.currenState.Clone(); 

                    foreach (var effect in action.Effects)
                    {

                        tempWorldState.ChangeWorldState(effect.Key, effect.Value);

                    }

                    GOAPNode newNode = new(tempWorldState, prevNode, action, prevNode.totalCost + action.Cost);

                    int matchingIndex = -1; 

                    for (int j = 0; j < savedWorldStates.Count; j++)
                    {

                        if (savedWorldStates[j].Item1.HasSameStates(newNode.currenState))
                        {

                            matchingIndex = j;
                            break; 

                        }

                    }

                    if (matchingIndex < 0)
                    {

                        nodeList.Add(newNode);
                        savedWorldStates.Add((tempWorldState, newNode.totalCost));

                    }
                    else if (savedWorldStates[matchingIndex].Item2 > newNode.totalCost)
                    {

                        nodeList.Add(newNode);
                        savedWorldStates[matchingIndex] = new(tempWorldState, newNode.totalCost);

                    }

                }

            }

        }

        Debug.Log("No plan can be found within search budget"); 

        return null; 

    }

    public bool CheckGoalAccomplished(GOAPGoal goals, WorldState worldState)
    {

        bool goalAchievable = true;

        foreach (var goal in goals.desiredStates)
        {

            if (!worldState.CheckWorldState(goal.Key, goal.Value)) { goalAchievable = false; break; }

        }

        Debug.Log("Goal accomplishable confirmed to be: " + goalAchievable); 

        return goalAchievable;

    }

    private void SetTotalCostOfPlan(GOAPNode finalNode)
    {

        totalCost = finalNode.totalCost; 

    }

    public float GetTotalCostOfPlan()
    {

        return totalCost; 

    }

    public Queue<GOAPAction> BacktrackActionsFromNodeList(GOAPNode finalNode)
    {

        if (finalNode == null || finalNode.previousAction == null) { return null; }

        Queue<GOAPAction> newQueue = new();
        Stack<GOAPAction> tempStack = new(); 

        GOAPNode currentNode = finalNode;

        while (currentNode != null && currentNode.previousAction != null)
        {

            tempStack.Push(currentNode.previousAction); 
            currentNode = currentNode.previousNode;

        }

        while (tempStack.Count > 0) 
        {
            newQueue.Enqueue(tempStack.Pop());

        }

        return newQueue; 

    }

    public GOAPNode FindCheapestNodeInList(List<GOAPNode> nodeList)
    {

        if (nodeList.Count == 0) return null; 

        float cheapestCost = nodeList[0].totalCost;

        int indexOfCheapest = 0; 

        for (int i = 0; i < nodeList.Count; i++)
        {

            if (nodeList[i].totalCost < cheapestCost)
            {

                cheapestCost = nodeList[i].totalCost;
                indexOfCheapest = i; 

            }

        }

        return nodeList[indexOfCheapest]; 

    }

    public bool IsOudatedNode(List<(WorldState, float)> savedWorldStates, GOAPNode prevNode)
    {

        bool isOudated = false;

        foreach (var savedState in savedWorldStates)
        {

            if (savedState.Item1.HasSameStates(prevNode.currenState) && savedState.Item2 < prevNode.totalCost)
            {

                isOudated = true;
                break;

            }

        }

        return isOudated; 

    }

}
