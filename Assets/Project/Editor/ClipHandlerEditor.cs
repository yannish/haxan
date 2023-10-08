using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClipHandler))]
public class ClipHandlerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var clipHandler = target as ClipHandler;

		using(new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("STATE:", EditorStyles.largeLabel);
			GUI.enabled = false;
			EditorGUILayout.FloatField("main weight", clipHandler.GetMainWeight());
			GUI.enabled = true;
		}
	}
}
