using System.Collections.Generic;
using UnityEngine;

//The actions as well as properties of the action like the preconditions for the action as well as the effects of the action on the world state
//and an abstract function for unique behaviors for the action

public enum ActionStatus
{
    Running, Success, Failure

}

[System.Serializable]
public abstract class GOAPAction 
{
    public float Cost = 1f; 
    public Dictionary<string, bool> Preconditions = new();
    public Dictionary<string, bool> Effects = new();

    [SerializeField]
    public List<WorldStateEntry> PreconditionsEntry = new();

    [SerializeField]
    public List<WorldStateEntry> EffectsEntry = new();
    public abstract ActionStatus Execute(GOAPAgent agent);
    public void InitializeDictionaries()
    {

        foreach (var entry in PreconditionsEntry)
        {

            Preconditions[entry.stateName] = entry.stateActive; 

        }
        
        foreach (var entry in EffectsEntry)
        {

            Effects[entry.stateName] = entry.stateActive; 

        }
        
    }
    
}
