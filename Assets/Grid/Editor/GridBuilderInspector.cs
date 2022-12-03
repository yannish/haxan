using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridBuilder))]
public class GridBuilderInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var gridBuilder = target as GridBuilder;

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("CREATE GRID"))
		{
			gridBuilder.CreateGrid();
		}
		if (GUILayout.Button("CLEAR GRID"))
		{
			gridBuilder.ClearGrid();
		}
		if (GUILayout.Button("BIND CELLOBJECTS"))
		{
			gridBuilder.BindAllCellObjects();
		}
		EditorGUILayout.EndHorizontal();

		//base.OnInspectorGUI();

		//if(GUILayout.Button("Check"))
		//{
		//	var result = gridBuilder.gridWidth / gridBuilder.gridHeight;
		//	Debug.Log("result: " + result.ToString());
		//}
	}
}
