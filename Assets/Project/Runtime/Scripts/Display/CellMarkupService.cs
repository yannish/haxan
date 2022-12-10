using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMarkupService : Service<CellMarkupService>
{
	//[Header("MARKERS:")]


	[Header("MOVE:")]
	public PooledMonoBehaviour moveMarker;
	public PooledMonoBehaviour jointMarker;
	public PooledMonoBehaviour arrowMarker;

	//[ReadOnly] public List<GameObject> placedMoveMarkers = new List<GameObject>();

	//[ReadOnly] public List<GameObject> placedJointMarkers = new List<GameObject>();


	public Action MarkMovePath(Cell startingCell, List<Cell> pathCells)
	{
		Action undo = () => { };

		HexDirection lastDir = startingCell.To(pathCells[0]);

		if (pathCells.Count == 1)
		{
			var newArrowMarker = arrowMarker.GetAndPlay(startingCell.transform.position, lastDir.ToVector());
			undo += () => newArrowMarker.gameObject.SetActive(false);
			return undo;
		}

		var firstMoveMarker = moveMarker.GetAndPlay(startingCell.transform.position, lastDir.ToVector());
		undo += () => firstMoveMarker.gameObject.SetActive(false);

		for (int i = 0; i < pathCells.Count - 1; i++)
		{
			Cell currCell = pathCells[i];
			Cell nextCell = pathCells[i + 1];

			if(currCell == null || nextCell == null)
			{
				Debug.LogWarning("move chain was broken!");
				return undo;
			}

			HexDirection toNextCellDir = currCell.To(nextCell);
			if(toNextCellDir != lastDir)
			{
				var newJointMarker = jointMarker.GetAndPlay(currCell.transform.position, toNextCellDir.ToVector());
				undo += () => newJointMarker.gameObject.SetActive(false);
			}

			if (i == pathCells.Count - 2)
			{
				var newArrowMarker = arrowMarker.GetAndPlay(currCell.transform.position, toNextCellDir.ToVector());
				undo += () => newArrowMarker.gameObject.SetActive(false);
			}
			else
			{
				var newMoverMarker = moveMarker.GetAndPlay(currCell.transform.position, toNextCellDir.ToVector());
				undo += () => newMoverMarker.gameObject.SetActive(false);
			}

			lastDir = toNextCellDir;
		}

		return undo;
	}


	[Header("PUSH:")]
	public PooledMonoBehaviour pushMarker;
	[ReadOnly] public List<GameObject> placedPushMarkers = new List<GameObject>();


    public Action MarkCellPush(Cell cell, HexDirection pushDir)
	{
		if (pushMarker == null)
			return null;

		Action markAction = null;

		Vector3 facingDir = pushDir.ToVector();

		var newMarker = pushMarker.GetAndPlay(
			cell.occupantPivot.position + Vector3.up * 0.2f,
			facingDir
			);

		placedPushMarkers.Add(newMarker.gameObject);

		return markAction;
	}

	public void ClearMarkers()
	{
		foreach (var placedMarker in placedPushMarkers)
		{
			placedMarker.SetActive(false);
		}

		placedPushMarkers.Clear();
	}
}
