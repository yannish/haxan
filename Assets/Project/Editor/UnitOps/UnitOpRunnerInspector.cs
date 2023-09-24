using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(UnitOpRunner))]
public class UnitOpRunnerInspector : Editor
{
	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		
	}

	UnitOpRunner opRunner;

	public override void OnInspectorGUI()
	{
		opRunner = target as UnitOpRunner;

		DrawDefaultInspector();

		if (Application.isPlaying)
		{
			using (new VerticalScope(EditorStyles.helpBox))
			{
				//for (int i = 0; i < 2; i++)
				for (int i = 0; i < UnitOpRunner.MAX_OPS; i++)
				{
					if(opRunner.allOps[i] == null)
					{
						EditorGUILayout.LabelField("NULL OP!");
						continue;
					}

					opRunner.allOps[i].DrawInspectorContext();
				}
			}
		}
	}
}
