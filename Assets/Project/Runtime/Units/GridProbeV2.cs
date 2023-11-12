using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridProbeV2 : MonoBehaviour
{
	GameObject hoverPrefab;
	private void Awake()
	{
		hoverPrefab = (GameObject)Resources.Load("Prefabs/BoardUI/HoveredCell");
	}


	[Header("PROBE:")]
	public int radius;
	public int min;
	public HexDirectionFT direction;
	//[ReadOnly] public 
	List<GameObject> grabMarkers = new List<GameObject>();

	[Header("PATHING:")]
	public Transform pathingDummy;

	public EditorButton grabValidBtn = new EditorButton("GrabValidNeighbours", true);
	public void GrabValidNeighbours()
	{
		Debug.LogWarning("GRABBING VALID NEIHGBOURS");	
		GrabCellsAt(this.transform.position.ToOffset().GetValidNeighbouringCoords());
	}

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
			radius,
			min
			));
	}

	public EditorButton grabCrossBtn = new EditorButton("GrabCross", true);
	public void GrabCross()
	{
		GrabCellsAt(
			HexDirectionV2.GetCardinalCross(Board.WorldToOffset(this.transform.position),
			radius,
			min
			));
	}

	public EditorButton grabCellBtn = new EditorButton("GrabCell", true);
	public Vector3Int cellGrabOffset;
	public void GrabCell()
	{
		Vector2Int offsetCoord = Board.WorldToOffset(this.transform.position);
		Debug.LogWarning("offset: " + offsetCoord);
		Vector3Int cubicCoor = Board.OffsetToCubic(offsetCoord);
		Debug.LogWarning("cubic: " + offsetCoord);
		Vector3Int newCubicCoord = cubicCoor + cellGrabOffset;
		Vector2Int newOffsetCoord = Board.CubicToOffset(newCubicCoord);

		GrabCellsAt(new List<Vector2Int> { newOffsetCoord });
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


	private void Start()
	{
		if(pathingDummy != null)
		{
			currDummyOffsetPos = pathingDummy.position.ToOffset();
			prevDummyOffsetPos = currDummyOffsetPos;
		}
	}

	public Vector2Int currDummyOffsetPos;
	public Vector2Int prevDummyOffsetPos;
	public void Update()
	{
		if (pathingDummy == null)
			return;

		prevDummyOffsetPos = currDummyOffsetPos;
		currDummyOffsetPos = pathingDummy.position.ToOffset();
		if (currDummyOffsetPos == prevDummyOffsetPos)
			return;

		var newPath = Board.FindPath_NEW(this.transform.position.ToOffset(), currDummyOffsetPos);
		if (newPath.IsNullOrEmpty())
		{
			Debug.LogWarning("NO PATH:");
			return;
		}

		Debug.LogWarning($"Path length: {newPath.Count()}");
		ReleaseGrabbedCells();
		GrabCellsAt(newPath.ToList());
	}
}
