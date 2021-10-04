using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Series : Compositor
{
    public override NodeStatus Tick()
    {
        if (!CheckConditions())
            return status;

        NodeStatus childStatus;

        for(int i = 0; i < children.Count; i++)
        {
            currNode = children[i];
            currNode.Enter();

            childStatus = children[i].Tick();

            if(childStatus == NodeStatus.FAILURE)
            {
                FinishUnsuccessfully();
                return status;
            }

            if(childStatus == NodeStatus.RUNNING)
            {
                status = childStatus;
                return status;
            }
        }

        FinishSuccesfully();

        return status;
    }
}
