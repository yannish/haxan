using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlowController : CharacterFlowController
{
    [ReadOnly] public Enemy enemy;
    [ReadOnly] public Blackboard blackboard;

  //  public override bool TryGetInputTurn(out Turn inputTurn)
  //  {
  //      inputTurn = null;

  //      enemy.tree.root.Tick();

		////if (!blackboard.currCommandStack.IsNullOrEmpty())
		////{
		////	//inputTurn = blackboard.currCommandStack;
		////	return true;
		////}

		//return false;
  //  }

    public override void BeginTurn()
    {
        base.BeginTurn();
    }

    public override void EndTurn()
    {
        //blackboard.currCommandStack = null;
    }
}
