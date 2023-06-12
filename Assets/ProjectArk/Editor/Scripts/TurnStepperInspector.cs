using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TurnStepper), true)]
public class TurnStepperInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var turnStepper = target as TurnStepper;

		EditorGUILayout.BeginHorizontal();

		if(GUILayout.Button("Step Forward"))
		{
			turnStepper.StepForward();
		}

		if (GUILayout.Button("Step Backward"))
		{
			turnStepper.StepBackward();
		}

		EditorGUILayout.EndHorizontal();
	}
}
