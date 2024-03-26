using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClipHandler_OLD))]
public class ClipHandlerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var clipHandler = target as ClipHandler_OLD;

		using(new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("STATE:", EditorStyles.largeLabel);
			GUI.enabled = false;
			EditorGUILayout.FloatField("main weight", clipHandler.GetMainWeight());
			GUI.enabled = true;
		}
	}
}
