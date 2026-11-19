using System.Collections.Generic;
using UnityEngine;

//The actions as well as properties of the action like the preconditions for the action as well as the effects of the action on the world state
//and an abstract function for unique behaviors for the action
public abstract class GOAPAction 
{

    public float Cost { get; protected set; } = 1f;

    public Dictionary<string, bool> Preconditions = new();

    public Dictionary<string, bool> Effects = new();
    public abstract void Execute(GOAPAgent agent);

}
