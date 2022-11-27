using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridProbe : CellObject
{
	[Space(10)]
	[Header("GRID TESTING:")]
    public int maxLength;

    public List<Cell> foundPath = new List<Cell>();

    private Action grabControl;

    public CellObject marker;

	public EditorButton checkDistance = new EditorButton("CheckDistance");
	public void CheckDistance()
	{
        Debug.Log("cube distance: " + this.currCell.CubeDistance(marker.currCell));
	}

	public EditorButton grabRing = new EditorButton("GrabRing", true);
	public int ringRadius;
	public void GrabRing()
	{
		if (Release())
			return;

		grabControl = CellActions.EffectCells<CellPathCommand>(
			this.currCell.GetCardinalRing(ringRadius, t => !t.IsBound())
			);
	}

	public EditorButton grabRadius = new EditorButton("GrabRadius", true);
	public int radius;
	public void GrabRadius()
	{
		if (Release()) 
			return;

		grabControl = CellActions.EffectCells<CellPathCommand>(
			this.currCell.GetCellsInRadius(
				radius, 
				t => !t.IsBound() && this.currCell.HasPathTo(t, maxLength)
				)
			);
	}

	bool Release()
	{
		if (grabControl != null)
		{
			grabControl.Invoke();
			grabControl = null;
			return true;
		}

		return false;
	}


	[ContextMenu("Try Path")]
    public void TryPath()
	{
  //      if(startCell.HasPathTo(destCell, out foundPath, maxLength))
		//{
  //          Debug.LogWarning("found a legit path!");
		//}
  //      else
		//{
  //          Debug.LogWarning("couldn't find a legit path!");
  //      }
    }
}
