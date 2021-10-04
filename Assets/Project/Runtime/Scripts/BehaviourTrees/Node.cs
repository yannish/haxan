using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NodeStatus
{
    RUNNING,
    FAILURE,
    SUCCESS
}

public abstract class Node : MonoBehaviour
{
    [ReadOnly] public NodeStatus status;
    [ReadOnly] public bool started = false;

    [ReadOnly] public Blackboard blackboard;
    public void ProvideBlackboard(Blackboard blackboard) { this.blackboard = blackboard; }

    public abstract NodeStatus Tick();
    public abstract Node Initialize(Blackboard blackboard);

    public virtual bool CheckConditions() { return status == NodeStatus.RUNNING; }

    public virtual void Enter()
    {
        status = NodeStatus.RUNNING;
        started = true;
    }

    public virtual void Exit() { }

    public void FinishSuccesfully()
    {
        started = false;
        status = NodeStatus.SUCCESS;
        Exit();
    }

    public void FinishUnsuccessfully()
    {
        started = false;
        status = NodeStatus.FAILURE;
        Exit();
    }
}
