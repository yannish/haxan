using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlowController : CharacterFlow
{
    [ReadOnly] public Enemy enemy;
    [ReadOnly] public Blackboard blackboard;

    public override bool TryGetCommandStack(ref Stack<CharacterCommand> commandStack)
    {
        enemy.tree.root.Tick();

        if (!blackboard.currCommandStack.IsNullOrEmpty())
        {
            commandStack = blackboard.currCommandStack;
            return true;
        }

        return false;
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
    }

    public override void EndTurn()
    {
        blackboard.currCommandStack = null;
    }
}
