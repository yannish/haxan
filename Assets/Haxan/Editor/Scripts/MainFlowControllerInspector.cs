using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MainFlowController))]
public class MainFlowControllerInspector : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorUtils.DrawScriptField(serializedObject);
		DrawContent();
		serializedObject.ApplyModifiedProperties();


	}

	public void OnEnable()
	{
		//Debug.Log("Enabled Control flow manager inspector");
		EditorApplication.update += DoRepaint;
	}

	public void OnDisable()
	{
		//Debug.Log("Disabled Control flow manager inspector");
		EditorApplication.update -= DoRepaint;
	}

	void DoRepaint()
	{
		//Debug.Log("Repainting");
		//MainFlowController mainFlow = target as MainFlowController;
		//mainFlow.ShowCurrentController();
		this.Repaint();
	}

	void DrawContent()
	{
		MainFlowController mainFlow = target as MainFlowController;
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("FLOW:", EditorStyles.boldLabel);
		GUI.enabled = false;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("peekedFlow"));//, new GUIContent("peeked flow: "));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("subFlow"));//, new GUIContent("sub flow: "));
		GUI.enabled = true;
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("MAIN:", EditorStyles.boldLabel);
		GUI.enabled = false;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("phase"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("currCharacter"));
		GUI.enabled = true;
		EditorGUILayout.Space();

		var origFontStyle = EditorStyles.label.fontStyle;
		EditorStyles.label.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("COMMAND STACK:");
		EditorStyles.label.fontStyle = origFontStyle;

		if (mainFlow.currCommandStack == null || mainFlow.currCommandStack.Count == 0)
		{
			EditorGUILayout.LabelField("...");
		}
		else
		{
			float labelWidth = EditorGUIUtility.labelWidth;

			EditorGUIUtility.labelWidth = 60f;

			var commandList = mainFlow.currCommandStack.ToList();

			for (int i = 0; i < mainFlow.currCommandStack.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(commandList[i].ToString());
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.Space();

		EditorStyles.label.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("COMMAND HISTORY:");
		EditorStyles.label.fontStyle = origFontStyle;

		if (mainFlow.commandHistory == null || mainFlow.commandHistory.Count == 0)
		{
			EditorGUILayout.LabelField("...");
		}
		else
		{
			float labelWidth = EditorGUIUtility.labelWidth;

			EditorGUIUtility.labelWidth = 60f;

			var commandList = mainFlow.commandHistory.ToList();

			for (int i = 0; i < mainFlow.commandHistory.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(commandList[i].ToString());
				EditorGUILayout.EndHorizontal();
			}
		}

	}
}
