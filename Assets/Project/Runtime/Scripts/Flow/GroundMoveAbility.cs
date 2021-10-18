using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GroundMoveAbility : Ability
{
	public FloatReference stepDuration;
	public FloatReference turnDuration;

	private Action pathControl;

    public override List<Cell> GetValidMoves(Cell cell, CharacterFlow flow)
	{
		return flow.character.CurrentCell.GetCellsInRadius(
			flow.character.maxMove, 
			t => !t.IsBound()
			);
	}

	public override void Peek(Cell targetCell, CharacterFlow flow)
	{
		var path = Pathfinder.GetPath(flow.character.CurrentCell, targetCell);
		if (path.IsNullOrEmpty())
			return;

		pathControl = CellActions.NewEffectCells<CellPathCommand>(path);
	}

	public override void Unpeek()
	{
		if(pathControl != null)
		{
			pathControl.Invoke();
			pathControl = null;
		}
	}

	public override Queue<CharacterCommand> FetchCommandChain(Cell targetCell, CharacterFlow flow)
	{
		if (targetCell == flow.character.CurrentCell || !targetCell.IsPassable || targetCell.IsBound())
			return null;

		var pathToCell = Pathfinder.GetPath(flow.character.CurrentCell, targetCell);
		if (pathToCell == null)
			return null;


		Queue<CharacterCommand> newCommandStack = new Queue<CharacterCommand>();
		HexDirection toFirstCellDir = flow.character.CurrentCell.To(pathToCell[0]);

		string log = string.Format("currFacing : {0} , toFirstCell : {1}", flow.character.facing, toFirstCellDir);
		Debog.logGameflow(log);

		if (flow.character.facing != toFirstCellDir)
		{
			TurnCommand newTurnCommand = new TurnCommand(flow, flow.character.facing, toFirstCellDir, turnDuration);
			newCommandStack.Enqueue(newTurnCommand);

			string firstTurnLog = string.Format("doing turn from : {0} , to : {1}", flow.character.facing, toFirstCellDir);
			Debog.logGameflow(firstTurnLog);
		}

		HexDirection lastFacingDirection = toFirstCellDir;
		for (int i = 0; i < pathToCell.Count; i++)
		{
			Cell fromCell = i == 0 ? flow.character.CurrentCell : pathToCell[i - 1];
			Cell toCell = pathToCell[i];
			HexDirection toNextCellDir = fromCell.To(toCell);
			if(lastFacingDirection != toNextCellDir)
			{
				TurnCommand newTurnCommand = new TurnCommand(
					flow, 
					lastFacingDirection, 
					toNextCellDir, 
					turnDuration
					);

				newCommandStack.Enqueue(newTurnCommand);
				lastFacingDirection = toNextCellDir;

				string turnLog = string.Format("turn from : {0} , to : {1}", flow.character.facing, toFirstCellDir);
				Debog.logGameflow(turnLog);
			}

			var newStepCommand = new StepCommand(flow, fromCell, toCell, stepDuration);
			newCommandStack.Enqueue(newStepCommand);

			string nextLog = string.Format("from : {0} , to : {1}", fromCell.name, toCell.name);
			Debog.logGameflow(nextLog);
		}

		return newCommandStack;
	}
}
