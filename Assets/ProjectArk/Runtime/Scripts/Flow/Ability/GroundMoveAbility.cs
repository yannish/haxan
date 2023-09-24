using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GroundMoveAbility : Ability_OLD
{
	public FloatReference stepDuration;
	public FloatReference turnDuration;

	private Action pathControl;

    public override List<Cell_OLD> GetValidMoves(Cell_OLD cell, CharacterFlowController flow)
	{
		return flow.character.currCell.GetCellsInRadius(
			flow.character.CurrMove,
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

	public override void Peek(Cell_OLD targetCell, CharacterFlowController flow)
	{
		var path = Pathfinder.GetPath(flow.character.currCell, targetCell);
		if (path.IsNullOrEmpty())
			return;

		pathControl = CellMarkupService.I.MarkMovePath(flow.character.currCell, path);
		//pathControl = CellActions.EffectCells<CellPathCommand>(path);
	}

	public override void Unpeek()
	{
		if(pathControl != null)
		{
			pathControl.Invoke();
			pathControl = null;
		}
	}

	public override Queue<CellObjectCommand> FetchCommandChain(
		Cell_OLD targetCell, 
		CellObject cellObj, 
		FlowController flow
		)
	{
		if (targetCell == cellObj.currCell || !targetCell.IsPassable || targetCell.IsBound())
			return null;

		var pathToCell = Pathfinder.GetPath(cellObj.currCell, targetCell);
		if (pathToCell == null)
			return null;

		//... TODO: this is getting rough. 
		if (!(flow is CharacterFlowController))
			return null;


		var characterFlow = flow as CharacterFlowController;

		Queue<CellObjectCommand> newCommands = new Queue<CellObjectCommand>();
		//Turn newTurn = base.FetchCommandChain(targetCell, cellObj, flow);

		HexDirection toFirstCellDir = cellObj.currCell.To(pathToCell[0]);

		string log = string.Format("currFacing : {0} , toFirstCell : {1}", cellObj.facing, toFirstCellDir);
		Debog.logGameflow(log);

		if (cellObj.facing != toFirstCellDir)
		{
			TurnCommand_OLD newTurnCommand = new TurnCommand_OLD(
				characterFlow.character, 
				cellObj.facing, 
				toFirstCellDir, 
				turnDuration
				);

			//newTurn.commands.Enqueue(newTurnCommand);
			newCommands.Enqueue(newTurnCommand);

			string firstTurnLog = string.Format(
				"doing turn from : {0} , to : {1}",
				cellObj.facing, 
				toFirstCellDir
				);

			Debog.logGameflow(firstTurnLog);
		}

		HexDirection lastFacingDirection = toFirstCellDir;
		for (int i = 0; i < pathToCell.Count; i++)
		{
			Cell_OLD fromCell = i == 0 ? cellObj.currCell : pathToCell[i - 1];
			Cell_OLD toCell = pathToCell[i];
			HexDirection toNextCellDir = fromCell.To(toCell);
			if(lastFacingDirection != toNextCellDir)
			{
				TurnCommand_OLD newTurnCommand = new TurnCommand_OLD(
					characterFlow.character, 
					lastFacingDirection, 
					toNextCellDir, 
					turnDuration
					);

				//newTurn.commands.Enqueue(newTurnCommand);
				newCommands.Enqueue(newTurnCommand);
				lastFacingDirection = toNextCellDir;

				string turnLog = string.Format("turn from : {0} , to : {1}", cellObj.facing, toFirstCellDir);
				Debog.logGameflow(turnLog);
			}

			var newStepCommand = new StepCommand(
				characterFlow.character, 
				fromCell, 
				toCell, 
				stepDuration
				);

			//newTurn.commands.Enqueue(newStepCommand);
			newCommands.Enqueue(newStepCommand);

			string nextLog = string.Format("from : {0} , to : {1}", fromCell.name, toCell.name);
			Debog.logGameflow(nextLog);
		}

		return newCommands;
	}
}
