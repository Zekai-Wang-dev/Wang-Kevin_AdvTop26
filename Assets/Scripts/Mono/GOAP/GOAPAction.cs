using System.Collections.Generic;
using UnityEngine;

public abstract class GOAPAction : MonoBehaviour
{

    public float Cost { get; protected set; } = 1f;

    public Dictionary<string, bool> Preconditions = new();

    public Dictionary<string, bool> Effects = new();
    public abstract void Execute(GOAPAgent agent);

}
