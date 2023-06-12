using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ScrObjAbility : ScriptableObject
{
    public Sprite icon;

    public virtual List<Cell_OLD> GetReachableCells(Cell_OLD cell) { return null; }
	public virtual List<Cell_OLD> GetTargetableCells(Cell_OLD cell)
	{
		return GetReachableCells(cell).Where(t => t.IsBound()).ToList();
	}
}
