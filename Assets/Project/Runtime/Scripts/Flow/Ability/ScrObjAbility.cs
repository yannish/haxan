using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ScrObjAbility : ScriptableObject
{
    public Sprite icon;

    public virtual List<Cell> GetReachableCells(Cell cell) { return null; }
	public virtual List<Cell> GetTargetableCells(Cell cell)
	{
		return GetReachableCells(cell).Where(t => t.IsBound()).ToList();
	}
}
