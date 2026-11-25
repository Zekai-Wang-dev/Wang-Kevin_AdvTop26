using System;
using System.Collections.Generic;
using System.Security;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class GOAPProfileLoader 
{

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

        List<Dictionary<string, string>> profileRows = CSVReader.ReadCSV(profileActionsTable);
        List<Dictionary<string, string>> preconditionsRows = CSVReader.ReadCSV(preconditionsDataTable);
        List<Dictionary<string, string>> effectsRows = CSVReader.ReadCSV(effectsDataTable);
        List<Dictionary<string, string>> dataRows = CSVReader.ReadCSV(actionsDataTable);

        foreach (var profile in profileRows)
        {

            if (profile["ProfileID"] != agent.ProfileID) continue;

            foreach (Type actionType in typeof(GOAPAction).Assembly.GetTypes())
            {

                GOAPAction action = (GOAPAction)Activator.CreateInstance(actionType);

                foreach (var preconditions in preconditionsRows)
                {

                    if (preconditions["ActionID"] != profile["ActionID"]) continue; 

                    string precondition = preconditions["WorldStateID"]; 
                    bool state = bool.Parse(preconditions["State"]);

                    action.Preconditions[precondition] = state; 

                }

                foreach (var effects in effectsRows)
                {

                    if (effects["ActionID"] != profile["ActionID"]) continue;

                    string effect = effects["WorldStateID"];
                    bool state = bool.Parse(effects["State"]); 

                    action.Effects[effect] = state;

                }

                foreach (var data in dataRows)
                {

                    if (data["ActionID"] != profile["ActionID"]) continue;

                    int cost = int.Parse(data["InitialCost"]);

                    action.Cost = cost; 

                }

                actions.Add(action);
            }

        }

    }

}
