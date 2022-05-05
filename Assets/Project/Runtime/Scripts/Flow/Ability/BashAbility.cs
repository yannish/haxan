using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BashAbility : Ability
{
	public Action bashControl;

	public override List<Cell> GetValidMoves(Cell cell, CharacterFlowController flow)
	{
		return cell.GetCardinalRing(1);
		//return base.GetValidMoves(cell, flow);
	}

	public override void Peek(Cell targetCell, CharacterFlowController flow)
	{
		var otherRing = targetCell.GetCardinalRing(1);
		bashControl = CellActions.EffectCells<CellPathCommand>(otherRing);
	}

	public override void Unpeek()
	{
		if(bashControl != null)
		{
			bashControl.Invoke();
			bashControl = null;
		}
	}
}
