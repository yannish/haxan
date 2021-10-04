using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Compositor : Node
{
    [ReadOnly] public Node currNode;


    private List<Node> _children;
    public List<Node> children
    {
        get
        {
            if (children.IsNullOrEmpty())
                _children = this.GetComponentsInDirectChildren<Node>();
            return _children;
        }
    }

    public override Node Initialize(Blackboard blackboard)
    {
        foreach(Node child in children)
        {
            child.Initialize(blackboard);
        }

        this.ProvideBlackboard(blackboard);

        return this;
    }
}
