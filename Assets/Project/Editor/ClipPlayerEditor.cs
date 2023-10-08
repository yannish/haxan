using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomEditor(typeof(ClipPlayer))]
public class ClipPlayerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var clipPlayer = target as ClipPlayer;

		DrawDefaultInspector();

		if (!Application.isPlaying)
			return;


		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("CLIPS:", EditorStyles.boldLabel);
			for (int i = 0; i < clipPlayer.scrubClips.Count; i++)
			{
				using (new GUILayout.VerticalScope(EditorStyles.helpBox))
				{
					var scrubClip = clipPlayer.scrubClips[i];
					using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
					{
						//PLAY / PAUSE:
						GUIContent icon =
							scrubClip.clipPlayable.GetPlayState() == PlayState.Paused ?
							EditorGUIUtility.IconContent("d_PlayButton On") :
							EditorGUIUtility.IconContent("d_PauseButton On");

						if (GUILayout.Button(icon, GUILayout.Width(24f)))
						{

						}

						if (GUILayout.Button((clipPlayer.clips[i].name).ToUpper()))
							clipPlayer.SetClip(i);
					}

					EditorGUI.BeginChangeCheck();
					float newScrubTime = EditorGUILayout.Slider(
						(float) scrubClip.clipPlayable.GetTime(),
						0f,
						1f
						);
					if (EditorGUI.EndChangeCheck())
					{
						clipPlayer.Scrub(i, newScrubTime);
					}
				}
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
