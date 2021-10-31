using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCube : CellObject
{
    public List<Cell> grabbedCells = new List<Cell>();

	protected override void Start()
	{
		base.Start();
		grabbedCells = currCell.GetCardinalRing(1);
	}
}
