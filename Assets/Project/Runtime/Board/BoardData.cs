using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardData : MonoBehaviour
	, ISerializationCallbackReceiver
{
	[System.Serializable]
	public class CellEntry
	{
		public int coordIndex;
		public Cell cell;

		public CellEntry(int index, Cell cell)
		{
			this.coordIndex = index;
			this.cell = cell;
		}
	}

	public List<GridV2> allGrids;

	public Dictionary<int, Cell> indexToCellLookup = new Dictionary<int, Cell>();

	public List<CellEntry> entries = new List<CellEntry>();

	//public List<List<Cell>> allCells;
	//public Cell[][] allCells;

	public EditorButton refreshBtn = new EditorButton("Refresh", false);

	private void OnEnable()
	{
		Debug.LogWarning("Enabled board data.");
	}

	public void OnBeforeSerialize()
	{
		//... put into serializable list
		//Debug.LogWarning("BEFORE SERIALIZING BOARD DATA");

		entries.Clear();

		foreach (var kvp in indexToCellLookup)
		{
			entries.Add(new(kvp.Key, kvp.Value));
		}
	}

	public void OnAfterDeserialize()
	{
		//... rebuild lookup
		Debug.LogWarning("AFTER SERIALIZING BOARD DATA");

		indexToCellLookup.Clear();
		foreach(CellEntry entry in entries)
		{
			indexToCellLookup.Add(entry.coordIndex, entry.cell);
		}
	}

	public void Refresh()
	{
		Debug.LogWarning("... refreshing board data");

		if(allGrids != null)
			allGrids.Clear();
		
		allGrids = FindObjectsOfType<GridV2>().ToList();
		if (allGrids.IsNullOrEmpty())
			return;

		//allCells = new List<List<Cell>>();
		foreach(var grid in allGrids)
		{
			var allCellsUnderGrid = grid.GetComponentsInChildren<Cell>().ToList();
			grid.cells = allCellsUnderGrid;
			//Debug.LogWarning("adding group of cels:" + allCellsUnderGrid.Count);
			//allCells.Add(allCellsUnderGrid);
		}

		indexToCellLookup.Clear();

		foreach(var grid in allGrids)
		{
			foreach(var cell in grid.cells)
			{
				var coord = Board.WorldToOffset(cell.transform.position);
				int index = coord.ToIndex();
				indexToCellLookup.Add(index, cell);
			}
		}
	}
}
