using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Puff", menuName = "Abilities/Puff")]
public class PuffAbilityScrObj : AbilityScrObj
{
	public override List<Cell_OLD> GetValidMoves(Cell_OLD cell, CharacterFlowController flow)
	{
		return cell.GetCardinalRing(1);
		//return base.GetValidMoves(cell, flow);
	}
}
