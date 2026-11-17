using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldStateEntry
{

    public string stateName;
    public bool stateActive; 

}

public class WorldState : MonoBehaviour
{

    private Dictionary<string, bool> states;

    private List<WorldStateEntry> entries;

    private void Start()
    {
        states = new Dictionary<string, bool>();

        InitializeDictionary();
    }

    public void InitializeDictionary()
    {

        states.Clear(); 

        foreach (var entry in entries)
        {

            states.Add(entry.stateName, entry.stateActive); 

        }

    }

    public void AddWorldState(bool active, string name)
    {

        WorldStateEntry newEntry = new WorldStateEntry();
        newEntry.stateActive = active;
        newEntry.stateName = name;

        entries.Add(newEntry);

        InitializeDictionary();

    }

    public bool CheckWorldState(string name)
    {

        states.TryGetValue(name, out bool stateActive);

        return stateActive; 
    }


}
