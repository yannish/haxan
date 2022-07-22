using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMarkupService : Service<CellMarkupService>
{
	[Header("MARKERS:")]
	[ReadOnly]
	public List<GameObject> placedMarkers = new List<GameObject>();

	public PooledMonoBehaviour pushMarker;
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

		placedMarkers.Add(newMarker.gameObject);

		return markAction;
	}

	public void ClearMarkers()
	{
		foreach (var placedMarker in placedMarkers)
		{
			placedMarker.SetActive(false);
		}

		placedMarkers.Clear();
	}
}
