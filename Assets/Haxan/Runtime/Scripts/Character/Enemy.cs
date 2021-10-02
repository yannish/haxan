using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public Blackboard blackboard;

    private BehaviourTree _tree;
    public BehaviourTree tree
    {
        get
        {
            if (!_tree)
                _tree = GetComponentInChildren<BehaviourTree>();
            return _tree;
        }
    }
}
