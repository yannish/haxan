using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMoveWithBounceAbility : Ability
{
	public FloatReference stepDuration;
	public FloatReference turnDuration;

	private Action pathControl;

	public override List<Cell_OLD> GetValidMoves(Cell_OLD cell, CharacterFlowController characterFlow)
	{
		List<Cell_OLD> allCellsInRange = characterFlow.character.currCell.GetCellsInRadius(
			characterFlow.character.CurrMove
			);

		List<Cell_OLD> deflectableCells = new List<Cell_OLD>();

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

		List <Cell_OLD> unboundCells = characterFlow.character.currCell.GetCellsInRadius(
			characterFlow.character.CurrMove,
			t => !t.IsBound()
			);



		return allCellsInRange;
	}
}
