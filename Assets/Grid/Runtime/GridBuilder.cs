using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(HexGrid))]
public class GridBuilder : MonoBehaviour
{
	[Header("Grid:")]
	[Range(0, 12)]
	public int gridWidth;
	[Range(0, 12)]
	public int gridHeight;

	[Header("Cells:")]
	public GameObject cellPrefab;
	public Cell[] cells
	{
		get { return hexGrid.cells; }
		set { hexGrid.cells = value; }
	}

	HexGrid hexGrid	{ get { return GetComponent<HexGrid>(); } }


	private void Awake()
	{
		hexGrid.coordCellLookup.Clear();
		for (int i = 0; i < cells.Length; i++)
		{
			//Debug.Log("Binding coord: " + cells[i].coords.ToString() + " to " + cells[i].name);
			hexGrid.coordCellLookup.Add(cells[i].coords, cells[i]);
		}
	}


	public void CreateGrid()
	{
		ClearGrid();

		cells = new Cell[gridHeight * gridWidth];
		for (int z = 0, i = 0; z < gridHeight; z++)
			for (int x = 0; x < gridWidth; x++)
				CreateCell(x, z, i++);

		//hexGrid.coordCellLookup.Clear();
		//for (int i = 0; i < cells.Length; i++)
		//{
		//	Debug.Log("Binding coord: " + cells[i].coords.ToString() + " to " + cells[i].name);
		//	hexGrid.coordCellLookup.Add(cells[i].coords, cells[i]);
		//}
	}

	public void BindAllCellObjects()
	{
		Globals.Grid.cellObjectBindings.Clear();

		CellObject[] existingCellObjects = FindObjectsOfType<CellObject>();
		foreach (var cellObj in existingCellObjects)
			cellObj.BindInPlace();
	}

	public void ClearGrid()
	{
		if (cells == null)
			return;

		for(int i = 0; i < cells.Length; i++)
		{
			if (
				cells[i] != null
				&& cells[i].gameObject
				)
				DestroyImmediate(cells[i].gameObject);
		}

		cells = null;
	}

	public void MakeOneCell()
	{
		var cellPrefabObj = PrefabUtility.InstantiatePrefab(cellPrefab) as GameObject;

		//CreateCell(0, 0, 0);
	}

	public GameObject randomPrefab;
	public void InstantiateAPrefab()
	{
		if(randomPrefab == null)
		{
			Debug.LogWarning("Assign a random prefab!");
			return;
		}

		var newPrefab = PrefabUtility.InstantiatePrefab(randomPrefab) as GameObject;
		if (newPrefab != null)
			Debug.LogWarning("Here's your new prefab: " + newPrefab.gameObject.name, newPrefab);
		else
			Debug.LogWarning("NUL!");
	}

	void CreateCell(int x, int z, int i)
	{
		Vector3 pos;

		pos.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		pos.y = 0f;
		pos.z = z * (HexMetrics.outerRadius * 1.5f);

		//Cell cell = cells[i] = Instantiate(cellPrefab).GetComponent<Cell>();

		var cellPrefabObj = PrefabUtility.InstantiatePrefab(cellPrefab) as GameObject;
		//return;

		Cell cell = cells[i] = cellPrefabObj.GetComponent<Cell>();
		if (cell == null)
			return;

		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = pos;
		cell.offsetCoords = new OffsetCoordinates(x, z);
		cell.coords = HexCoordinates.FromOffsetCoordinates(x, z);
		//cell.coords = cell.offsetCoords.ToCubeCoords();
		cell.name = string.Concat("hexCell: ",cell.coords.ToString());
		//... if cell isn't on western-most edge, it has a westward neighbour:
		if (x > 0)
			cell.SetNeighbour(cells[i - 1], HexDirection.W);

		//... if cell isn't on the southern-most edge...
		if (z > 0)
		{
			//... and it's on an even row...
			if((z & 1) == 0)
			{
				//... it has a south-eastern neighbour on the previous row:
				cell.SetNeighbour(cells[i - gridWidth], HexDirection.SE);

				//... if it's not on the western-most edge, it also has a south-western neighbour on the previous row:
				if (x > 0)
					cell.SetNeighbour(cells[i - gridWidth - 1], HexDirection.SW);
			}
			else
			{
				//... otherwise it's on an odd row, so it has a south-western neighbour on the previous row:
				cell.SetNeighbour(cells[i - gridWidth], HexDirection.SW);

				//... if it's not on the eastern-most edge, it has a south-eastern neighbour on the previous row:
				if (x < gridWidth - 1)
					cell.SetNeighbour(cells[i - gridWidth + 1], HexDirection.SE);
			}
		}
	}
}
