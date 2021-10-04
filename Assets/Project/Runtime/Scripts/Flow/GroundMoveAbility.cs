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
    public override List<Cell> GetValidMoves(Cell cell, Character character)
	{
		List<Cell> validCells = new List<Cell>();

		validCells = character.CurrentCell.GetCardinalRing(character.maxMove, t => !t.IsBound());

		return null;
	}

	public override List<CharacterCommand> FetchCommandChain(Cell targetCell, Character character)
	{
		if (targetCell == character.CurrentCell || !targetCell.IsPassable)
			return null;

		var pathToCell = Pathfinder.GetPath(character.CurrentCell, targetCell);
		if (pathToCell == null)
			return null;

		List<CharacterCommand> newCommandStack = new List<CharacterCommand>();

		for (int i = 0; i < pathToCell.Count; i++)
		{
			Cell fromCell = i == 0 ? character.CurrentCell : pathToCell[i - 1];

			var newStepCommand = new StepCommand(character, fromCell, pathToCell[i], 1f);
			newCommandStack.Add(newStepCommand);

			Debug.Log("pushing step command " + newStepCommand.ToString());
		}

		return newCommandStack;
	}
}
