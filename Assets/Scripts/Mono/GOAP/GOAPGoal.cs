using System.Collections.Generic;
using UnityEngine;

//The goal of the agent used to determine the plan of actions 
public class GOAPGoal 
{
    
    public string goalName;
    public int priority;
    public Dictionary<string, bool> desiredStates; 

}
