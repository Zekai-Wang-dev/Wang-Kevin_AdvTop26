using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//An entry for the world state that can be accessed in properties and put into a dictionary 
public class WorldStateEntry
{

    public string stateName;
    public bool stateActive; 

}

//The world state of the owner 
public class WorldState : MonoBehaviour
{

    private Dictionary<string, bool> states = new(); 

    private List<WorldStateEntry> entries;

    private void Start()
    {

        InitializeDictionary();
    }

    //Initialize the dictionary based on the entries made in the inspector 
    public void InitializeDictionary()
    {

        states.Clear(); 

        foreach (var entry in entries)
        {

            states.Add(entry.stateName, entry.stateActive); 

        }

    }

    //A fuction to add new world states 
    public void AddWorldState(bool active, string name)
    {

        WorldStateEntry newEntry = new WorldStateEntry();
        newEntry.stateActive = active;
        newEntry.stateName = name;

        entries.Add(newEntry);

        InitializeDictionary();

    }

    //A function to check a specific world state from the dictionary 
    public bool CheckWorldState(string name)
    {

        states.TryGetValue(name, out bool stateActive);

        return stateActive; 

    }

    //A fuction to change a state in the list of world states. 
    public void ChangeWorldState(string name, bool stateActive)
    {

        states[name] = stateActive;

    }

    public Dictionary<string, bool> GetWorldStates()
    {

        return states;
    }

    public WorldState Clone()
    {

        WorldState newWorldState = new(); 


        foreach (var state in states)
        {

            newWorldState.AddWorldState(state.Value, state.Key); 

        }

        return newWorldState; 

    }

}
