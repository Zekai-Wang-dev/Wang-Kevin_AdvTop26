using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[System.Serializable]
//An entry for the world state that can be accessed in properties and put into a dictionary 
public class WorldStateEntry
{

    public string stateName;
    public bool stateActive; 

}

//The world state of the owner 
[System.Serializable]
public class WorldState 
{

    private Dictionary<string, bool> states = new();

    private ReadOnlyDictionary<string, bool> readOnlyStates; 

    [SerializeField]
    private List<WorldStateEntry> InitialStateEntries = new(); 

    //Initialize the dictionary based on the entries made in the inspector 
    public void InitializeDictionary()
    {

        states.Clear(); 

        foreach (var entry in InitialStateEntries)
        {

            states[entry.stateName] = entry.stateActive; 

        }

    }

    //A function to check a specific world state from the dictionary 
    public bool CheckWorldState(string name, bool requiredValue)
    {

        return states.TryGetValue(name, out bool stateActive) && stateActive == requiredValue;

    }

    //A fuction to change a state in the list of world states. 
    public void ChangeWorldState(string name, bool stateActive)
    {

        states[name] = stateActive;

    }

    public IReadOnlyDictionary<string, bool> GetWorldStates()
    {

        readOnlyStates ??= new ReadOnlyDictionary<string, bool>(states);

        return readOnlyStates;
    }

    public WorldState Clone()
    {

        WorldState newWorldState = new(); 


        foreach (var state in states)
        {

            newWorldState.ChangeWorldState(state.Key, state.Value); 

        }

        return newWorldState; 

    }

    public bool HasSameStates(WorldState otherWorldState)
    {

        IReadOnlyDictionary<string, bool> otherStates = otherWorldState.GetWorldStates();

        if (otherStates.Count != states.Count) return false; 

        foreach (var state in states)
        {


            if (!otherStates.TryGetValue(state.Key, out bool active) || state.Value != active)
            {

                return false;

            }

        }

        return true; 

    }

}
