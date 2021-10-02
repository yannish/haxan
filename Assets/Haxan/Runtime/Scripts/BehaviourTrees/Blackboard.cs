using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    public GameObject agent { get; private set; }

    public Stack<CharacterCommand> currCommandStack;

    public Blackboard(GameObject agent)
    {
        this.agent = agent;
        currCommandStack = new Stack<CharacterCommand>();
    }
}
