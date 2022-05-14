using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomahawkAbility : Ability
{
    public override List<Cell> GetValidMoves(Cell cell, CharacterFlowController flow)
	{
		return cell.GetCardinalRing(2);
	}
}
