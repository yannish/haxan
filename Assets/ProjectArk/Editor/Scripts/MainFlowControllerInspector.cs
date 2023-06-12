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
		
		DrawFlowHierarchy();

		DrawPlaybackControls();

		serializedObject.ApplyModifiedProperties();
	}

	MainFlowController mainFlow;

	public void OnEnable()
	{
		Debug.Log("Enabled Control flow manager inspector");
		
		EditorApplication.update -= Repaint;
		EditorApplication.update += Repaint;

		mainFlow = target as MainFlowController;

		mainFlow.turnProcessor = null;

		var grabbedProcessors = mainFlow.GetComponentsInChildren<ITurnProcessor>()
			.Where(t => t is Component && (t as Component).gameObject.activeSelf)
			.ToList();

		if(!grabbedProcessors.IsNullOrEmpty())
			mainFlow.turnProcessor = grabbedProcessors.FirstOrDefault();

		mainFlow.serialTurnSystem = mainFlow.GetComponentInChildren<SerialTurnSystem>();

		serializedObject.ApplyModifiedProperties();
	}

	public void OnDisable() => EditorApplication.update -= Repaint;

	void DrawFlowHierarchy()
	{
		flowHeirarchy.Clear();
		EditorGUILayout.LabelField("FLOW HEIRARCHY:", EditorStyles.boldLabel);

		FlowController flowToDraw = mainFlow;
		while (flowToDraw != null)
		{
			flowHeirarchy.Add(flowToDraw);
			flowToDraw = flowToDraw.subFlow;
		}

		foreach (var flow in flowHeirarchy)
		{
			string label = string.Format("- {0}", flow.name);
			EditorGUILayout.ObjectField(new GUIContent(label), flow, typeof(FlowController), true);
			EditorGUI.indentLevel++;
		}

		foreach (var flow in flowHeirarchy)
		{
			EditorGUI.indentLevel--;
		}
		EditorGUILayout.Space();

		GUI.enabled = false;
		EditorGUILayout.LabelField("FLOW DEBUG:", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("peekedFlow"));//, new GUIContent("peeked flow: "));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("subFlow"));//, new GUIContent("sub flow: "));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("lastSubFlow"));//, new GUIContent("sub flow: "));
		GUI.enabled = true;
		EditorGUILayout.Space();

		//EditorGUILayout.LabelField("MAIN:", EditorStyles.boldLabel);
		//GUI.enabled = false;
		//EditorGUILayout.PropertyField(serializedObject.FindProperty("phase"));
		//GUI.enabled = true;


		EditorGUILayout.Space();
	}


	List<FlowController> flowHeirarchy = new List<FlowController>();
	void DrawContent()
	{
		MainFlowController mainFlow = target as MainFlowController;
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("logDebug"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("doBreak"));
		EditorGUILayout.Space();

		//var origFontStyle = EditorStyles.label.fontStyle;
		//EditorStyles.label.fontStyle = FontStyle.Bold;
		//EditorGUILayout.LabelField("COMMAND STACK:");
		//EditorStyles.label.fontStyle = origFontStyle;
		//EditorGUILayout.Space();

		//EditorStyles.label.fontStyle = FontStyle.Bold;
		//EditorGUILayout.LabelField("COMMAND HISTORY:");
		//EditorStyles.label.fontStyle = origFontStyle;
	}


	private Editor serialTurnEditor = null;
	void DrawPlaybackControls()
	{
		EditorGUILayout.LabelField("TURN PROCESSING:", EditorStyles.boldLabel);

		if(mainFlow != null && mainFlow.serialTurnSystem != null)
		{
			if (mainFlow.serialTurnSystem.IsProcessing)
				GUI.enabled = false;

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("UNDO TURN"))
			{
				mainFlow.serialTurnSystem.Undo();
			}
			if (GUILayout.Button("REDO TURN"))
			{
				mainFlow.serialTurnSystem.Redo();
			}
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
		}

		SerializedProperty serialTurnSystemProp = serializedObject.FindProperty("serialTurnSystem");
		EditorGUILayout.PropertyField(serialTurnSystemProp);
		EditorGUILayout.Space();

		if (serialTurnSystemProp.objectReferenceValue != null)
		{
			using (var horizontalScope = new GUILayout.HorizontalScope())
			{
				GUILayout.Space(20f); // horizontal indent size od 20 (pixels?)
				using (var verticalScope = new GUILayout.VerticalScope())
				{
					if (!serialTurnEditor)
						Editor.CreateCachedEditor(
							serialTurnSystemProp.objectReferenceValue,
							null,
							ref serialTurnEditor
							);
					serialTurnEditor.OnInspectorGUI();
					// anything you do in here will be indented by 20 pixels
					// relative to stuff outside the top using( xxx) scope
				}
			}
		}

		//if (mainFlow.turnProcessor != null && (mainFlow.turnProcessor as Component) != null)
		//{
		//	var turnProcessorObj = (mainFlow.turnProcessor as Component).gameObject;
		//	EditorGUILayout.ObjectField(
		//		new GUIContent("Turn Processor"),
		//		turnProcessorObj,
		//		typeof(GameObject),
		//		true
		//		);
		//}
		//else
		//{
		//	EditorGUILayout.HelpBox(
		//		"NO TURN PROCESSOR FOUND!",
		//		MessageType.Warning,
		//		true
		//		);
		//}
	}
}
