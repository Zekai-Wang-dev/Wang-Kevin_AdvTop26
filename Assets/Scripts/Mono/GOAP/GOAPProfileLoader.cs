using System;
using System.Collections.Generic;
using UnityEngine;

public class GOAPProfileLoader : MonoBehaviour
{

    public static GOAPProfileLoader Instance { get; private set; }

    [Header("Profile Tables")]
    [SerializeField] private TextAsset profileIDTable;
    [SerializeField] private TextAsset profileActionsTable;
    [SerializeField] private TextAsset profileGoalsTable;
    [SerializeField] private TextAsset profileInitialStates;

    [Header("Action Tables")]
    [SerializeField] private TextAsset actionsDataTable;
    [SerializeField] private TextAsset preconditionsDataTable;
    [SerializeField] private TextAsset effectsDataTable;

    [Header("Goal and State Tables")]
    [SerializeField] private TextAsset goalsDataTable;
    [SerializeField] private TextAsset goalConditions;
    [SerializeField] private TextAsset worldStateID;

    public void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void LoadProfile(GOAPAgent agent)
    {

        LoadGoals(agent);
        LoadActions(agent);
        LoadWorldStates(agent);

    }
    public void LoadWorldStates(GOAPAgent agent)
    {

        WorldState state = new();

        List<Dictionary<string, string>> rows = CSVReader.ReadCSV(profileInitialStates); 

        foreach (var row in rows)
        {

            if (row["ProfileID"] != agent.ProfileID) continue;

            string stateID = row["WorldStateID"];
            bool stateValue = bool.Parse(row["State"]); 

            state.ChangeWorldState(stateID, stateValue);

        }

        agent.currentState = state;

    }

    public void LoadGoals(GOAPAgent agent)
    {

        List<GOAPGoal> goals = new();

        List<Dictionary<string, string>> profileRows = CSVReader.ReadCSV(profileGoalsTable);
        List<Dictionary<string, string>> dataRows = CSVReader.ReadCSV(goalsDataTable);
        List<Dictionary<string, string>> conditionsRows = CSVReader.ReadCSV(goalConditions);

        foreach (var profile in profileRows)
        {

            if (profile["ProfileID"] != agent.ProfileID) continue;

            GOAPGoal goal = new();

            string goalName = profile["GoalID"];

            goal.goalName = goalName;

            foreach (var conditions in conditionsRows)
            {

                if (conditions["GoalID"] != goalName) continue; 

                string condition = conditions["WorldStateID"];
                bool state = bool.Parse(conditions["State"]); 

                goal.desiredStates.Add(condition, state); 

            }

            foreach (var priorities in dataRows)
            {

                if (priorities["GoalID"] != goalName) continue;

                int priority = int.Parse(priorities["Priority"]); 
                goal.priority = priority;

            }

            goals.Add(goal);
            
        }

        agent.goals = goals;

    }

    public void LoadActions(GOAPAgent agent)
    {

        List<GOAPAction> actions = new();

        var profileRows = CSVReader.ReadCSV(profileActionsTable);
        var preconditionsRows = CSVReader.ReadCSV(preconditionsDataTable);
        var effectsRows = CSVReader.ReadCSV(effectsDataTable);
        var dataRows = CSVReader.ReadCSV(actionsDataTable);

        foreach (var profile in profileRows)
        {

            if (profile["ProfileID"] != agent.ProfileID) continue;

            string actionID = profile["ActionID"];

            foreach (var data in dataRows)
            {

                if (data["ActionID"] != actionID) continue;

                string className = data["ActionType"]; 

                foreach (Type actionType in typeof(GOAPAction).Assembly.GetTypes())
                {
                    
                    if (className != actionType.Name) continue;

                    if (!typeof(GOAPAction).IsAssignableFrom(actionType)) continue;

                    if (actionType.IsAbstract) continue;

                    GOAPAction action = (GOAPAction)Activator.CreateInstance(actionType);

                    action.Cost = float.Parse(data["InitialCost"], System.Globalization.CultureInfo.InvariantCulture);

                    foreach (var precondition in preconditionsRows)
                    {
                        if (precondition["ActionID"] != actionID) continue;

                        string stateName = precondition["WorldStateID"];
                        bool stateValue = bool.Parse(precondition["State"]);

                        action.Preconditions[stateName] = stateValue;

                    }

                    foreach (var effect in effectsRows)
                    {
                        if (effect["ActionID"] != actionID) continue;

                        string stateName = effect["WorldStateID"];
                        bool stateValue = bool.Parse(effect["State"]);

                        action.Effects[stateName] = stateValue;

                    }


                    actions.Add(action);
                    break; 

                }

                break; 

            }

            agent.actions = actions;

        }

    }

}
