using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Tick children in order.
- Return running while any child is still running.
- Return success if EACH child has finished successfully.
- Return failure if ANY child fails.
*/

public class BTSequence : Compositor
{
    public override NodeStatus Tick()
    {
        if (!CheckConditions())
            return status;

        NodeStatus currChildStatus = currNode.Tick();
        int i = children.IndexOf(currNode);

        switch(currChildStatus)
        {
            case NodeStatus.SUCCESS:
                if (i == children.Count - 1)
                {
                    FinishSuccesfully();
                }
                else
                {
                    currNode = children[i + 1];
                    currNode.Enter();
                    status = NodeStatus.RUNNING;
                }
                break;

            case NodeStatus.RUNNING:
                status = NodeStatus.RUNNING;
                break;

            case NodeStatus.FAILURE:
                FinishUnsuccessfully();
                break;
        }

        return status;
    }

    public override void Enter()
    {
        base.Enter();
        currNode = children[0];
        currNode.Enter();
    }
}
