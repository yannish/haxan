using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomahawkAbility : Ability_OLD
{
    public override List<Cell_OLD> GetValidMoves(Cell_OLD cell, CharacterFlowController flow)
	{
		return cell.GetCardinalRing(2);
	}
}
