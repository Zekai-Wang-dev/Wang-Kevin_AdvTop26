using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//The agent's properties that include its avaiable actions, its goals, as well as the planenr its current world state. 
public class GOAPAgent : MonoBehaviour
{

    [SerializeField]
    public WorldState currentState = new WorldState();

    [SerializeField]
    public GOALPlanner goalPlanner = new GOALPlanner();

    [SerializeField, Min(1)]
    private int maxNodeExpansions = 20;

    [SerializeReference]
    public List<GOAPAction> actions = new(); 

    [SerializeField]
    public List<GOAPGoal> goals = new();
    private List<GOAPGoal> candidateGoals = new(); 
    private List<(GOAPGoal goal, Queue<GOAPAction> action, float totalCost)> candidatePlans = new();
    private (GOAPGoal goal, Queue<GOAPAction> action, float totalCost) chosenPlan = new();

    private void Start()
    {
        
        currentState.InitializeDictionary();

        foreach (var goal in goals)
        {

            goal.InitializeDictionary(); 

        }

        foreach (var action in actions)
        {

            action.InitializeDictionaries();

        }

        PlanActions(); 

    }

    private void Update()
    {
        if (chosenPlan.action != null && chosenPlan.action.Count > 0)
        {
            ExecuteNextAction(chosenPlan);
        }
    }

    public void PlanActions()
    {

        chosenPlan = default;

        if (goals.Count == 0) return;

        FindGoalCandidates();

        List<GOAPGoal> remainingGoals = candidateGoals;

        while (remainingGoals.Count > 0)
        {

            int highestPriority = int.MinValue;

            foreach (var goal in remainingGoals)
            {

                if (goal.priority > highestPriority)
                {

                    highestPriority = goal.priority;

                }

            }

            FindPlanCandidates(highestPriority, remainingGoals);

            if (candidatePlans.Count == 0)
            {

                remainingGoals.RemoveAll(goal => goal.priority == highestPriority);

                continue;

            }
            else
            {
                chosenPlan = GetLowestCostPlanFromCandidates();
                break; 

            }
        }

    }

    public (GOAPGoal goal, Queue<GOAPAction> action, float totalCost) GetLowestCostPlanFromCandidates()
    {

        float lowestCost = float.PositiveInfinity;
        int planIndex = 0; 

        for (int i = 0; i < candidatePlans.Count; i++) 
        {

            if (candidatePlans[i].totalCost < lowestCost)
            {

                lowestCost = candidatePlans[i].totalCost;
                planIndex = i; 

            }

        }

        return candidatePlans[planIndex]; 

    }

    public void FindGoalCandidates()
    {

        candidateGoals.Clear(); 

        foreach (var goal in goals)
        {

            if (!goalPlanner.CheckGoalAccomplished(goal, currentState))
            {

                candidateGoals.Add(goal);

            }

        }

    }

    public void FindPlanCandidates(int highestPriority, List<GOAPGoal> remainingGoals)
    {

        candidatePlans.Clear(); 

        foreach (var goal in remainingGoals)
        {

            if (goal.priority == highestPriority)
            {

                Queue<GOAPAction> plannedActions = goalPlanner.Plan(currentState, actions, goal, maxNodeExpansions);

                if (plannedActions != null)
                {

                    candidatePlans.Add((goal, plannedActions, goalPlanner.GetTotalCostOfPlan()));

                }

            }

        }

    }

    public void ExecuteNextAction((GOAPGoal goal, Queue<GOAPAction> actions, float totalCost) ChosenPlan)
    {

        GOAPAction action = ChosenPlan.actions.Peek();

        ActionStatus status = action.Execute(this); 

        switch(status)
        {
            case ActionStatus.Running:

                break;

            case ActionStatus.Failure:
                PlanActions(); 

                break;

            case ActionStatus.Success:

                ApplyActionEffects(action);
                ChosenPlan.actions.Dequeue();

                if (chosenPlan.action.Count == 0)
                {

                    PlanActions(); 

                }

                break; 

        }

    }

    public void ApplyActionEffects(GOAPAction action)
    {

        foreach(var effect in action.Effects)
        {

            currentState.ChangeWorldState(effect.Key, effect.Value); 

        }

    }

}
