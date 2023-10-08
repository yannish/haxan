using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomEditor(typeof(ClipTester))]
public class ClipTesterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var clipTester = target as ClipTester;

		DrawDefaultInspector();

		if (!Application.isPlaying)
			return;

		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			for (int i = 0; i < clipTester.clips.Count; i++)
			{
				GUIContent icon = EditorGUIUtility.IconContent("d_PlayButton On");

				using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
				{
					if(GUILayout.Button(icon, GUILayout.Width(24f)))
					{
						clipTester.clipHandler.SetClip(clipTester.clips[i]);
					}

					GUILayout.Button(clipTester.clips[i].name.ToUpper());
				}
			}
		}

		if(clipTester.clipHandler.currClipHandle != null)
		{
			EditorGUI.BeginChangeCheck();
			var newTime = EditorGUILayout.Slider(
				"Clip time:",
				(float)clipTester.clipHandler.currClipHandle.clipPlayable.GetTime(), 
				0f, 
				1f
				);
			if (EditorGUI.EndChangeCheck())
			{
				clipTester.time = newTime;
				clipTester.clipHandler.Scrub(newTime);
			}

			EditorGUI.BeginChangeCheck();
			var newWeight = EditorGUILayout.Slider(
				"Master weight:",
				(float)clipTester.clipHandler.playableOutput.GetWeight(),
				0f,
				1f
				);
			if (EditorGUI.EndChangeCheck())
			{
				clipTester.clipHandler.SetMainWeight(newWeight);
			}
		}

	}
}
