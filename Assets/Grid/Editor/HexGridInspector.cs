using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGrid))]
public class HexGridInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		HexGrid grid = target as HexGrid;

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Report Bindings"))
		{
			if (grid.cellObjectBindings == null)
				return;

			string log = string.Format(
				"keys: {0}, values: {1}",
				grid.cellObjectBindings.forwardLookup.Count,
				grid.cellObjectBindings.backwardLookup.Count
				);

			Debug.LogWarning("forward: ");
			foreach (var cell in grid.cellObjectBindings.forwardLookup.Keys)
			{
				Debug.LogWarning("... " + cell.name);
			}
			Debug.LogWarning("backward: ");
			foreach (var cellObj in grid.cellObjectBindings.backwardLookup.Keys)
			{
				if(cellObj == null)
				{
					Debug.LogAssertion("... missing cellObj in bindings");
					continue;
				}

				Debug.LogWarning("... " + cellObj.name);
			}
		}

		if(GUILayout.Button("Clear Binding"))
		{
			Undo.RecordObject(grid, "Clear Bindings");
			grid.cellObjectBindings.Clear();
		}
		EditorGUILayout.EndHorizontal();

	}
}
