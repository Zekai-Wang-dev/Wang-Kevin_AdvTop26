using UnityEngine;

[System.Serializable]
public class MoveToPoint : GOAPAction
{

    public string stateName = "WalkToPoint";

    private bool started;

    public bool ForceFail = false; 
    public override ActionStatus Execute(GOAPAgent agent)
    {

        var fsm = agent.GetComponent<NodeCanvas.StateMachines.FSMOwner>();
        var bb = agent.GetComponent<NodeCanvas.Framework.Blackboard>();

        if (fsm == null)
            return ActionStatus.Failure;

        if (!fsm.isRunning)
        {
            Debug.LogError("Cannot move: FSM is not running.");
            return ActionStatus.Failure;
        }

        // Wait until the FSM has entered its initial state.
        if (fsm.GetCurrentState(false) == null)
        {
            return ActionStatus.Running;
        }


        if (bb == null)
            return ActionStatus.Failure;

        if (!started)
        {
            bb.SetVariableValue("arrivedAtPoint", false);

            if (fsm.TriggerState(stateName) == null)
                return ActionStatus.Failure;

            started = true;
        }

        if (!bb.GetVariableValue<bool>("arrivedAtPoint"))
            return ActionStatus.Running;

        started = false;

        if (ForceFail)
            return ActionStatus.Failure;

        return ActionStatus.Success;

    }
}
