using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCube : CellObject
{
    public List<Cell_OLD> grabbedCells = new List<Cell_OLD>();

	protected override void Start()
	{
		base.Start();
		grabbedCells = currCell.GetCardinalRing(1);
	}
}
