using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Bash", fileName = "Bash")]
public class Bash : ScrObjAbility, ICellCommandDispenser
{
	public CellCommand GetCellCommand(Cell cell)
	{
		return new CellPathCommand();
	}

	public override List<Cell> GetReachableCells(Cell cell)
	{
		return cell.GetCardinalRing(2);
	}
}
