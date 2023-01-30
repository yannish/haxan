using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridProbeV2 : Unit
{

	GameObject hoverPrefab;
	private void Awake()
	{
		hoverPrefab = (GameObject)Resources.Load("Prefabs/HoveredCell");
	}



	[Header("PROBE:")]
	public int radius;
	public HexDirectionFT direction;
	//[ReadOnly] public 
	List<GameObject> grabMarkers = new List<GameObject>();


	public EditorButton grabRadiusBtn = new EditorButton("GrabRadius", true);
	public void GrabRadius()
	{
		Debug.LogWarning("GRABBING RADIUS");
		GrabCellsAt(HexDirectionV2.GetCellsInRadius(Board.WorldToOffset(this.transform.position), radius));
	}

	public EditorButton grabRingBtn = new EditorButton("GrabRing", true);
	public void GrabRing()
	{
		GrabCellsAt(
			HexDirectionV2.GetCardinalRing(Board.WorldToOffset(this.transform.position),
			radius
			));
	}

	public EditorButton grabLineBtn = new EditorButton("GrabLine", true);
	public void GrabLine()
	{
		GrabCellsAt(
			HexDirectionV2.GetCardinalLine(Board.WorldToOffset(this.transform.position),
			direction,
			radius
			));
	}


	private void GrabCellsAt(List<Vector2Int> offsetCoords)
	{
		ReleaseGrabbedCells();

		foreach (var offsetCoord in offsetCoords)
		{
			Vector3 worldPos = Board.OffsetToWorld(offsetCoord);
			
			var grabMarker = Instantiate(
				hoverPrefab,
				worldPos + Vector3.up * 0.2f,
				Quaternion.identity
				);

			//grabMarker.transform.SetParent(this.transform);
			grabMarkers.Add(grabMarker);
		}
	}

	public EditorButton releaseBtn = new EditorButton("ReleaseGrabbedCells", true);
	private void ReleaseGrabbedCells()
	{
		foreach(var gameObject in grabMarkers)
		{
			Destroy(gameObject);
		}

		grabMarkers.Clear();
	}
}
