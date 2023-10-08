using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(UnitOpRunner))]
public class UnitOpRunnerInspector : Editor
{
	private void OnEnable() { }

	private void OnDisable() { }

	UnitOpRunner opRunner;

	public override void OnInspectorGUI()
	{
		opRunner = target as UnitOpRunner;

		DrawDefaultInspector();

		if (!Application.isPlaying)
			return;


		using (new HorizontalScope())
		{
			if (GUILayout.Button("CLEAR"))
			{
				opRunner.ClearOps();
			}
			if (GUILayout.Button("SET"))
			{
				opRunner.SetOps();
			}
		}

		using (new HorizontalScope())
		{
			if (GUILayout.Button("SAVE"))
			{
				opRunner.SaveToJSON();
			}
			if (GUILayout.Button("LOAD"))
			{
				opRunner.LoadFromJSON();
			}
		}

		using (new VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("OPS:", EditorStyles.boldLabel);

			//for (int i = 0; i < 2; i++)
			for (int i = 0; i < UnitOpRunner.MAX_OPS; i++)
			{
				if(opRunner.allOps[i] == null)
				{
					//EditorGUILayout.LabelField("NULL OP!");
					continue;
				}

				opRunner.allOps[i].DrawInspectorContent();
			}
		}
	}
}
