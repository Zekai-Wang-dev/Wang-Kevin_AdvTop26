using System.Collections.Generic;
using UnityEngine;

//The goal of the agent used to determine the plan of actions 
[System.Serializable]
public class GOAPGoal 
{
    
    public string goalName;
    public int priority;
    private bool isCompleted; 
    public Dictionary<string, bool> desiredStates = new();

    [SerializeField]
    public List<WorldStateEntry> entryStates = new(); 

    public void InitializeDictionary()
    {

        foreach (var state in entryStates)
        {

            desiredStates[state.stateName] = state.stateActive; 

        }

    }

    public void SetComplete(bool newStatus)
    {

        isCompleted = newStatus; 

    }

    public bool IsCompleted()
    {

        return isCompleted;
    }

}
