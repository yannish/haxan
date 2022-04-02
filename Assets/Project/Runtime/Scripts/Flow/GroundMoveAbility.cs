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
		return flow.character.currCell.GetCellsInRadius(
			flow.character.currMove,
			t => !t.IsBound() && flow.character.currCell.HasPathTo(t, flow.character.maxMoves)
			);

		//return flow.character.currCell.GetCellsInRadius(
		//	flow.character.maxMove,
		//	t => !t.IsBound() && cell.HasPathTo(t)
		//	);

		//List<Cell> pathableCells = new List<Cell>();

		//for (int i = 1; i <= flow.character.maxMove; i++)
		//{
		//	if(i == 1)
		//	{
		//		var firstRing = flow.character.currCell.GetCardinalRing(
		//			1, 
		//			t => !t.IsBound()
		//			);

		//		if(!pathableCells.IsNullOrEmpty())
		//			pathableCells.AddRange(firstRing);

		//		continue;
		//	}

		//	var ring = flow.character.currCell.GetCardinalRing(
		//		i, 
		//		t => !t.IsBound() && cell.HasPathTo(t)
		//		);

		//	if (!ring.IsNullOrEmpty())
		//		pathableCells.AddRange(ring);
		//}

		//return pathableCells;


	}

	public override void Peek(Cell targetCell, CharacterFlow flow)
	{
		var path = Pathfinder.GetPath(flow.character.currCell, targetCell);
		if (path.IsNullOrEmpty())
			return;

		pathControl = CellActions.EffectCells<CellPathCommand>(path);
	}

	public override void Unpeek()
	{
		if(pathControl != null)
		{
			pathControl.Invoke();
			pathControl = null;
		}
	}

	public override Turn FetchCommandChain(Cell targetCell, CharacterFlow flow)
	{
		if (targetCell == flow.character.currCell || !targetCell.IsPassable || targetCell.IsBound())
			return null;

		var pathToCell = Pathfinder.GetPath(flow.character.currCell, targetCell);
		if (pathToCell == null)
			return null;


		Turn newTurn = base.FetchCommandChain(targetCell, flow);

		HexDirection toFirstCellDir = flow.character.currCell.To(pathToCell[0]);

		string log = string.Format("currFacing : {0} , toFirstCell : {1}", flow.character.facing, toFirstCellDir);
		Debog.logGameflow(log);

		if (flow.character.facing != toFirstCellDir)
		{
			TurnCommand newTurnCommand = new TurnCommand(flow, flow.character.facing, toFirstCellDir, turnDuration);
			newTurn.commands.Enqueue(newTurnCommand);

			string firstTurnLog = string.Format(
				"doing turn from : {0} , to : {1}", 
				flow.character.facing, 
				toFirstCellDir
				);
			Debog.logGameflow(firstTurnLog);
		}

		HexDirection lastFacingDirection = toFirstCellDir;
		for (int i = 0; i < pathToCell.Count; i++)
		{
			Cell fromCell = i == 0 ? flow.character.currCell : pathToCell[i - 1];
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

				newTurn.commands.Enqueue(newTurnCommand);
				lastFacingDirection = toNextCellDir;

				string turnLog = string.Format("turn from : {0} , to : {1}", flow.character.facing, toFirstCellDir);
				Debog.logGameflow(turnLog);
			}

			var newStepCommand = new StepCommand(flow, fromCell, toCell, stepDuration);
			newTurn.commands.Enqueue(newStepCommand);

			string nextLog = string.Format("from : {0} , to : {1}", fromCell.name, toCell.name);
			Debog.logGameflow(nextLog);
		}

		return newTurn;
	}
}
