using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/GroundMove", fileName = "GroundMove")]
public class GroundMoveAbilityScrObj : AbilityScrObj
{
    public FloatReference stepDuration;
    public FloatReference turnDuration;


	public override List<Cell_OLD> GetValidMoves(Cell_OLD cell, CharacterFlowController flow)
	{
		return flow.character.currCell.GetCellsInRadius(
			flow.character.CurrMove,
			t => !t.IsBound() && flow.character.currCell.HasPathTo(t, flow.character.maxMoves)
			);
	}


	public override Action Peek(Cell_OLD targetCell, CharacterFlowController flow)
	{
		Action peekControl;

		var path = Pathfinder_OLD.GetPath(flow.character.currCell, targetCell);
		if (path.IsNullOrEmpty())
			return null;

		peekControl = CellMarkupService.I.MarkMovePath(flow.character.currCell, path);

		return peekControl;
	}


	public override Queue<CellObjectCommand> FetchCommandChain(
		Cell_OLD targetCell,
		CellObject cellObj,
		FlowController flow
		)
	{
		if (targetCell == cellObj.currCell || !targetCell.IsPassable || targetCell.IsBound())
			return null;

		var pathToCell = Pathfinder_OLD.GetPath(cellObj.currCell, targetCell);
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
			if (lastFacingDirection != toNextCellDir)
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
