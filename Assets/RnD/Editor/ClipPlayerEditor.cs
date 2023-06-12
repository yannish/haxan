using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClipPlayer))]
public class ClipPlayerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var clipPlayer = target as ClipPlayer;

		DrawDefaultInspector();

		if (!Application.isPlaying)
			return;

		EditorGUILayout.LabelField("CLIPS:", EditorStyles.boldLabel);

		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			for (int i = 0; i < clipPlayer.clips.Count; i++)
			{
				if (GUILayout.Button((clipPlayer.clips[i].name).ToUpper()))
					clipPlayer.SetClip(i);
			}

			//EditorGUILayout.LabelField("CELL LOOKUP: ", EditorStyles.boldLabel);
			//foreach (var kvp in boardData.indexToCellLookup)
			//{
			//	GUI.enabled = false;
			//	using (new GUILayout.HorizontalScope())
			//	{
			//		EditorGUILayout.IntField(kvp.Key);
			//		EditorGUILayout.ObjectField(kvp.Value, typeof(Cell), true);
			//	}
			//	GUI.enabled = true;
			//}
		}
	}

	void DrawClip()
	{

	}
}
