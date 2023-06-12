using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SerialTurnSystem))]
public class SerialTurnSystemInspector : Editor
{
	private void OnEnable() => EditorApplication.update += Repaint;

	private void OnDisable() => EditorApplication.update -= Repaint;

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		EditorUtils.DrawScriptField(serializedObject);

		//DrawControls();

		DrawState();
		
		DrawCommandHistory();

		//DrawCommandStack();

		serializedObject.ApplyModifiedProperties();
	}

	private void DrawControls()
	{
		SerialTurnSystem system = target as SerialTurnSystem;

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("UNDO TURN"))
		{
			system.Undo();
		}
		if (GUILayout.Button("REDO TURN"))
		{
			system.Redo();
		}
		EditorGUILayout.EndHorizontal();
	}

	private void DrawState()
	{
		EditorGUILayout.Space();
		SerialTurnSystem system = target as SerialTurnSystem;

		EditorGUILayout.LabelField("STATE:", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("currPlaybackState"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("currPhase"));
	}

	private void DrawCommandHistory()
	{
		EditorGUILayout.Space();
		SerialTurnSystem system = target as SerialTurnSystem;

		//var origFontStyle = EditorStyles.label.fontStyle;
		//EditorStyles.label.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("COMMAND HISTORY:", EditorStyles.boldLabel);

		GUI.enabled = false;
		if (!system.turnHistory.IsNullOrEmpty())
		{
			foreach (var turn in system.turnHistory)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.ObjectField("instigator: ", turn.instigator, typeof(CellObject), true);
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel++;
				foreach (var command in turn.commandHistory)
				{
					command.DrawInspectorContent();
					EditorGUILayout.Space(5);
					//EditorGUILayout.LabelField(command.ToString());
				}
				EditorGUI.indentLevel--;
			}
		}
		else
		{
			EditorGUILayout.LabelField("...");
		}
		GUI.enabled = true;

		//if (system.commandHistory != null && !system.currTurn.commands.IsNullOrEmpty())
		//{
		//	float labelWidth = EditorGUIUtility.labelWidth;
		//	EditorGUIUtility.labelWidth = 60f;
		//	var commandList = system.currTurn.commands.ToList();
		//}
		//else
		//{
		//	EditorGUILayout.LabelField("...");
		//}
	}



	//private void DrawCommandStack()
	//{
	//	EditorGUILayout.Space();
	//	SerialTurnSystem system = target as SerialTurnSystem;

	//	//var origFontStyle = EditorStyles.label.fontStyle;
	//	//EditorStyles.label.fontStyle = FontStyle.Bold;
	//	EditorGUILayout.LabelField("COMMAND STACK:", EditorStyles.boldLabel);
	//	//EditorStyles.label.fontStyle = origFontStyle;
	//	if (system.currTurn != null && !system.currTurn.commands.IsNullOrEmpty())
	//	{
	//		float labelWidth = EditorGUIUtility.labelWidth;
	//		EditorGUIUtility.labelWidth = 60f;
	//		var commandList = system.currTurn.commands.ToList();

	//		for (int i = 0; i < system.currTurn.commands.Count; i++)
	//		{
	//			EditorGUILayout.BeginHorizontal();
	//			EditorGUILayout.LabelField(commandList[i].ToString());
	//			EditorGUILayout.EndHorizontal();
	//		}
	//	}
	//	else
	//	{
	//		EditorGUILayout.LabelField("...");
	//	}
	//}
}
