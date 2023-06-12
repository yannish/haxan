using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardData))]
public class BoardDataEditor : Editor
{
	BoardData boardData;

	public override void OnInspectorGUI()
	{
		serializedObject.DrawScriptField();

		boardData = target as BoardData;
		if (GUILayout.Button("REFRESH"))
			boardData.Refresh();

		DrawLookup();
	}

	void DrawLookup()
	{
		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("CELL LOOKUP: ", EditorStyles.boldLabel);
			foreach (var kvp in boardData.indexToCellLookup)
			{
				GUI.enabled = false;
				using (new GUILayout.HorizontalScope())
				{
					EditorGUILayout.IntField(kvp.Key);
					EditorGUILayout.ObjectField(kvp.Value, typeof(Cell), true);
				}
				GUI.enabled = true;
			}
		}
	}
}
