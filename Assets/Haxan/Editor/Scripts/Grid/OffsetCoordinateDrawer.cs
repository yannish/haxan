using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OffsetCoordinates))]
public class OffsetCoordinateDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//OffsetCoordinates coordinates = new OffsetCoordinates(
		//	property.FindPropertyRelative("column").intValue,
		//	property.FindPropertyRelative("row").intValue
		//	);

		//position = EditorGUI.PrefixLabel(position, label);
		//GUI.Label(position, coordinates.ToString());
		int column = property.FindPropertyRelative("column").intValue;
		int row = property.FindPropertyRelative("row").intValue;
		OffsetCoordinates coords = new OffsetCoordinates(column, row);
		GUI.Label(position, coords.ToString());
	}
}
