using UnityEngine;
using UnityEngine.Rendering;

public class GOAPNode 
{

    public GOAPNode(WorldState newState, GOAPNode newPrevNode, GOAPAction newPrevAction, float newTotalCost)
    {

        currenState = newState;
        previousNode = newPrevNode;
        previousAction = newPrevAction;
        totalCost = newTotalCost; 

    }

    public WorldState currenState; 
    public GOAPNode previousNode;
    public GOAPAction previousAction;
    public float totalCost; 

}
