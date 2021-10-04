using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Decorator : Node
{
    private Node _child;
    public Node child
    {
        get
        {
            if (!_child)
            {
                var children = this.GetComponentsInDirectChildren<Node>();
                if (!children.IsNullOrEmpty())
                    _child = children[0];
            }
            return _child;
        }
    }

    public override Node Initialize(Blackboard blackboard)
    {
        child.Initialize(blackboard);
        this.ProvideBlackboard(blackboard);
        return this;
    }


    public override void Enter()
    {
        base.Enter();
        child.Enter();
    }


    public override NodeStatus Tick()
    {
        status = child.Tick();
        return status;
    }
}
