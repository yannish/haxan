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

	MainFlowController mainFlow;

	public void OnEnable()
	{
		Debug.Log("Enabled Control flow manager inspector");

		EditorApplication.update += DoRepaint;

		mainFlow = target as MainFlowController;

		//var availableTurnProcessors = new List<>

		mainFlow.turnProcessor = null;
		//mainFlow.turnProcessors.Clear();

		var grabbedProcessors = mainFlow.GetComponentsInChildren<ITurnProcessor>()
			.Where(t => t is Component && (t as Component).gameObject.activeSelf)
			.ToList();

		if(!grabbedProcessors.IsNullOrEmpty())
		{
			//mainFlow.turnProcessors = grabbedProcessors.Select(t => (t as Component).gameObject).ToList();
			if (!grabbedProcessors.IsNullOrEmpty())
			{
				mainFlow.turnProcessor = grabbedProcessors.FirstOrDefault();
			}
		}

		serializedObject.ApplyModifiedProperties();

		//mainFlow.turnProcessor = mainFlow.GetComponentInChildren<ITurnProcessor>();
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

		EditorGUILayout.PropertyField(serializedObject.FindProperty("logDebug"));
		EditorGUILayout.Space();



		EditorGUILayout.LabelField("FLOW:", EditorStyles.boldLabel);
		GUI.enabled = false;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("peekedFlow"));//, new GUIContent("peeked flow: "));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("subFlow"));//, new GUIContent("sub flow: "));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("lastSubFlow"));//, new GUIContent("sub flow: "));
		GUI.enabled = true;
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("MAIN:", EditorStyles.boldLabel);
		GUI.enabled = false;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("phase"));
		GUI.enabled = true;

		if(mainFlow.turnProcessor != null && (mainFlow.turnProcessor as Component) != null)
		{
			var turnProcessorObj = (mainFlow.turnProcessor as Component).gameObject;
			EditorGUILayout.ObjectField(
				new GUIContent("Turn Processor"), 
				turnProcessorObj, 
				typeof(GameObject), 
				true
				);
		}
		else
		{
			EditorGUILayout.HelpBox(
				"NO TURN PROCESSOR FOUND!",
				MessageType.Warning,
				true
				);
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty("currInputTurn"));
		EditorGUILayout.Space();

		var origFontStyle = EditorStyles.label.fontStyle;
		EditorStyles.label.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("COMMAND STACK:");
		EditorStyles.label.fontStyle = origFontStyle;

		if (mainFlow.currInputTurn == null || mainFlow.currInputTurn.commands.IsNullOrEmpty())
		{
			EditorGUILayout.LabelField("...");
		}
		else
		{
			float labelWidth = EditorGUIUtility.labelWidth;

			EditorGUIUtility.labelWidth = 60f;

			var commandList = mainFlow.currInputTurn.commands.ToList();

			for (int i = 0; i < mainFlow.currInputTurn.commands.Count; i++)
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

		if(Application.isPlaying && !mainFlow.playerTurns.IsNullOrEmpty())
		{
			if(GUILayout.Button("RESOLVE"))
			{
				//mainFlow.SetPhase(TeamPhase.RESOLVE);
			}
		}
	}
}
