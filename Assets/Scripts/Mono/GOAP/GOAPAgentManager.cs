using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GOAPAgentEntry
{
    public GameObject agent;
    public Transform spawnLocation; 
    private int agentID;
    public int GetAgentID()
    {
        return agentID;
    }
    public void SetAgentID(int id)
    {
        agentID = id;
    }   
}

public class GOAPAgentManager : MonoBehaviour
{

    public List<GOAPAgentEntry> agentEntries = new List<GOAPAgentEntry>();

    public int agentId = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        SpawnAllAgents();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnAllAgents()
    {

        foreach (var entry in agentEntries)
        {
            GameObject newAgent = Instantiate(entry.agent, entry.spawnLocation.position, entry.spawnLocation.rotation);
            newAgent.name = $"{entry.agent.name}_{agentId}";
            entry.SetAgentID(agentId);
            agentId++;
        }

    }
}
