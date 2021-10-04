using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : MonoBehaviour
{
    private Node _root;
    public Node root
    {
        get
        {
            if (!_root)
                _root = GetComponentInChildren<Node>();
            return _root;
        }
    }
    
    public void Initialize(Blackboard blackboard)
    {
        root.Initialize(blackboard);
        root.Enter();
    }
}


