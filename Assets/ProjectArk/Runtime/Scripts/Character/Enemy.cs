using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Enemy : Character
{
    public int turnPriority;

    protected override void OnEnable()
    {
        Globals.ActiveEnemies?.Add(this);
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        Globals.ActiveEnemies?.Remove(this);
        base.OnDisable();
    }



    [ReadOnly] public Blackboard blackboard;

    [ReadOnly] public BehaviourTree _tree;
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
