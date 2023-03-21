using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellSwatch))]
public class CellSwatchEditor : Editor
{
	public override void OnInspectorGUI()
	{
		//CellSwatch swatch = target as CellSwatch;

		DrawDefaultInspector();

		//EditorGUILayout.EnumPopup(swatch.cellState);
		//EditorGUILayout.EnumFlagsField(swatch.cellState);

	}
}
