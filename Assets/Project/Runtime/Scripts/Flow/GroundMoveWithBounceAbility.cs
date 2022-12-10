using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMoveWithBounceAbility : Ability
{
	public FloatReference stepDuration;
	public FloatReference turnDuration;

	private Action pathControl;

	public override List<Cell> GetValidMoves(Cell cell, CharacterFlowController characterFlow)
	{
		List<Cell> allCellsInRange = characterFlow.character.currCell.GetCellsInRadius(
			characterFlow.character.currMove
			);

		List<Cell> deflectableCells = new List<Cell>();

		foreach(var inRangeCell in allCellsInRange)
		{
			if(inRangeCell.TryGetBoundCellObject(out var boundCellObject))
			{
				if(boundCellObject.preset.deflectionProfile != null)
				{
					if (
						boundCellObject.preset.deflectionProfile.TryDeflect(
							boundCellObject,
							cell.To(inRangeCell),
							out var deflectionDir
							))
					{

					}
				}
			}
		}

		List <Cell> unboundCells = characterFlow.character.currCell.GetCellsInRadius(
			characterFlow.character.currMove,
			t => !t.IsBound()
			);



		return allCellsInRange;
	}
}
