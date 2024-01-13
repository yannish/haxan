using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomEditor(typeof(ClipPlayer))]
public class ClipPlayerEditor : Editor
{
	enum PlayBackMode
	{

	}

	GUIContent playForwards;
	GUIContent playBackwards;
	GUIContent playFromStart;
	GUIContent rewindFromEnd;
	GUIContent blendTo;
	GUIContent pause;

	float buttonWidth = 24f;

	private void OnEnable()
	{
		var clipPlayer = target as ClipPlayer;
		playFromStart = EditorGUIUtility.IconContent("Animation.NextKey");
		rewindFromEnd = EditorGUIUtility.IconContent("Animation.PrevKey");
		playBackwards = EditorGUIUtility.IconContent("Profiler.PrevFrame");
		playForwards = EditorGUIUtility.IconContent("Profiler.NextFrame");
		blendTo = EditorGUIUtility.IconContent("UnityEditor.Graphs.AnimatorControllerTool");
		pause = EditorGUIUtility.IconContent("d_PauseButton On");

		GraphVisualizerClient.Show(clipPlayer.graph);
	}

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
						bool isPlayingForward = scrubClip.clipPlayable.GetPlayState() == PlayState.Playing;
						bool isReversing = scrubClip.clipPlayable.GetSpeed() < 0f;

						switch (scrubClip.mode)
						{
							case ScrubClipPlaybackMode.PAUSED:
								if(GUILayout.Button(playForwards, GUILayout.Width(buttonWidth)))
									clipPlayer.PlayClip(clipPlayer.scrubClips[i]);

								if(clipPlayer.currClip != clipPlayer.scrubClips[i])
								{
									if (GUILayout.Button(blendTo, GUILayout.Width(buttonWidth)))
										clipPlayer.TransitionTo(clipPlayer.scrubClips[i]);
								}
								break;

							case ScrubClipPlaybackMode.PLAYING:
								break;

							case ScrubClipPlaybackMode.REWINDING:
								break;
						}

						//... 1st icon:
						var firstIcon = playForwards;
						if (isPlayingForward && !isReversing)
							firstIcon = pause;

						//if (GUILayout.Button(icon, GUILayout.Width(buttonWidth)))
						//{

						//}

						//PLAY / PAUSE:
						//GUIContent icon =
						//	scrubClip.clipPlayable.GetPlayState() == PlayState.Paused ?
						//	EditorGUIUtility.IconContent("d_PlayButton On") :
						//	EditorGUIUtility.IconContent("d_PauseButton On");

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
						//clipPlayer.Scrub(i, newScrubTime);
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
