using UnityEngine;

[System.Serializable]
public class PickUpItem : GOAPAction
{
    public override ActionStatus Execute(GOAPAgent agent)
    {

        Debug.Log("Executing PickUpItem action for agent: " + agent.name);

        return ActionStatus.Success;
    }
}
