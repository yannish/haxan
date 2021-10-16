using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMoveAbilityFlow : FlowController
{
	public List<Cell> validMoves;

	public override void Enter()
	{
		
	}

	public override void Exit()
	{

	}
}

public class GroundMoveAbility : Ability
{
    public override List<Cell> GetValidMoves(Cell cell, CharacterFlow flow)
	{
		List<Cell> validCells = new List<Cell>();

		validCells = flow.character.CurrentCell.GetCardinalRing(
			flow.character.maxMove, 
			t => !t.IsBound()
			);

		return null;
	}

	public override List<CharacterCommand> FetchCommandChain(Cell targetCell, CharacterFlow flow)
	{
		if (targetCell == flow.character.CurrentCell || !targetCell.IsPassable)
			return null;

		var pathToCell = Pathfinder.GetPath(flow.character.CurrentCell, targetCell);
		if (pathToCell == null)
			return null;

		List<CharacterCommand> newCommandStack = new List<CharacterCommand>();

		for (int i = 0; i < pathToCell.Count; i++)
		{
			Cell fromCell = i == 0 ? flow.character.CurrentCell : pathToCell[i - 1];

			var newStepCommand = new StepCommand(flow, fromCell, pathToCell[i], 1f);
			newCommandStack.Add(newStepCommand);

			Debug.Log("pushing step command " + newStepCommand.ToString());
		}

		return newCommandStack;
	}
}
