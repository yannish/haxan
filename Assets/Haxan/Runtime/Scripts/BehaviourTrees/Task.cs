using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task : Node
{
    [ReadOnly] public float elapsedTime = 0f;
    [ReadOnly] public Character agent;

    public override void Enter()
    {
        elapsedTime = 0f;
        base.Enter();
    }

    public override bool CheckConditions()
    {
        if (GameState.wanderersCleared)
            return false;

        return base.CheckConditions();
    }

    public override Node Initialize(Blackboard blackboard)
    {
        ProvideBlackboard(blackboard);
        return this;
    }

    public override NodeStatus Tick()
    {
        elapsedTime += Time.deltaTime;
        return status;
    }
}
