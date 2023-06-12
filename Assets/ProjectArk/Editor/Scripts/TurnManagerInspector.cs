using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(TurnManager))]
public class TurnManagerInspector : Editor
{
	private ReorderableList list;

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorUtils.DrawScriptField(serializedObject);
		DrawContent();
		serializedObject.ApplyModifiedProperties();
	}

	void DrawScriptField()
	{
		GUI.enabled = false;
		SerializedProperty prop = serializedObject.FindProperty("m_Script");
		EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
		GUI.enabled = true;
	}

	void DrawContent()
	{
		TurnManager turnManager = target as TurnManager;

		if(turnManager.currCommandChain == null || turnManager.currCommandChain.Count == 0)
		{
			EditorGUILayout.LabelField("NO COMMANDS");
			return;
		}

		float labelWidth = EditorGUIUtility.labelWidth;

		EditorGUIUtility.labelWidth = 60f;

		var commandList = turnManager.currCommandChain.ToList();

		for (int i = 0; i < turnManager.currCommandChain.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(commandList[i].ToString());
			EditorGUILayout.EndHorizontal();
		}
	}
}
