using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cell))]
public class CellInspector : Editor
{
	private HexGrid foundHexGrid;
	private void OnEnable()
	{
		foundHexGrid = Globals.Grid;
	}

	public override void OnInspectorGUI()
	{
		if (!foundHexGrid)
			EditorGUILayout.LabelField("DIDN'T FIND GRID");

		Cell cell = target as Cell;

		if (Globals.Grid != null && cell.TryGetBoundCellObject(out CellObject foundCellObj))
		{
			var obj = foundCellObj.gameObject;
			EditorGUILayout.ObjectField("BOUND OBJECT: ", obj, typeof(Object), true);
		}

		DrawDefaultInspector();
	}
}
