using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : Decorator
{
    public override NodeStatus Tick()
    {
        status = child.Tick();

        if (status == NodeStatus.SUCCESS || status == NodeStatus.FAILURE)
        {
            child.Enter();
            status = NodeStatus.RUNNING;
        }

        return status;
    }
}
