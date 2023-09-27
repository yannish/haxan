using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMarkupService : Service<CellMarkupService>
{
	[Header("MOVE:")]
	public PooledMonoBehaviour moveMarker;
	public PooledMonoBehaviour jointMarker;
	public PooledMonoBehaviour arrowMarker;
	public PooledMonoBehaviour dashMarker;
	//[ReadOnly] public List<GameObject> placedMoveMarkers = new List<GameObject>();
	//[ReadOnly] public List<GameObject> placedJointMarkers = new List<GameObject>();

	public Action MarkDashPath(Cell_OLD startingCell, List<Cell_OLD> pathCells)
	{
		Action undo = () => { };

		for (int i = 0; i < pathCells.Count - 1; i++)
		{
			Cell_OLD currCell = pathCells[i];
			Cell_OLD nextCell = pathCells[i + 1];
			if (currCell == null || nextCell == null)
			{
				Debug.LogWarning("move chain was broken!");
				return undo;
			}

			HexDirection toNextCellDir = currCell.To(nextCell);
			var newDashMarker = dashMarker.GetAndPlay(
				currCell.transform.position,
				toNextCellDir.ToVector()
				);

			undo += () => newDashMarker.gameObject.SetActive(false);
		}

		return undo;
	}

	public Action MarkMovePath(Cell_OLD startingCell, List<Cell_OLD> pathCells)
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
			Cell_OLD currCell = pathCells[i];
			Cell_OLD nextCell = pathCells[i + 1];

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


    public Action MarkCellPush(Cell_OLD cell, HexDirection pushDir)
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
