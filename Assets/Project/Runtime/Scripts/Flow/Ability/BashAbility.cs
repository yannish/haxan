using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BashAbility : Ability_OLD
{
	public override List<Cell_OLD> GetValidMoves(Cell_OLD cell, CharacterFlowController flow)
	{
		return cell.GetCardinalRing(1);
		//return base.GetValidMoves(cell, flow);
	}



	//public override void Peek(Cell targetCell, CharacterFlowController flow)
	//{
	//	//var otherRing = targetCell.GetCardinalRing(1);
	//	var peekedCells = GetValidMoves(targetCell, flow);

	//	var pushDirection = flow.character.currCell.To(targetCell);

	//	CellMarkupService.I?.MarkCellPush(targetCell, pushDirection);

	//	peekAction = CellActions.EffectCells<CellPathCommand>(peekedCells);
	//}

	//public override void Unpeek()
	//{
	//	if(peekAction != null)
	//	{
	//		peekAction.Invoke();
	//		peekAction = null;
	//	}
	//}
}
