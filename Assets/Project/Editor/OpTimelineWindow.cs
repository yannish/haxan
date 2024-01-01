using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OpTimelineWindow : EditorWindow
{
	//public static Action onScrubToTurnClicked;
	//public static Action onSmashCutToTurnClicked;

	int selectedTurnIndex = -1;
	Vector2 scrollPos;
	Vector2 debugScrollPos;
	private const int debugTurnCount = 20;
	public float currSliderValue;
	public float currFloatSliderValue;

	GUIContent playIcon;
	GUIContent undoIcon;
	GUIContent ffIcon;


	[MenuItem("Haxan/Timeline")]
	public static void ShowWindow()
	{
		OpTimelineWindow window = (OpTimelineWindow)GetWindow(typeof(OpTimelineWindow));
		window.Show();
	}

	private void Awake()
	{
		Debug.LogWarning("AWAKE ON OP TIMELINE");
	}

	private void OnEnable() => EditorApplication.playModeStateChanged += HandlePlaymodeChange;

	private void OnDisable() => EditorApplication.playModeStateChanged -= HandlePlaymodeChange;

	private void HandlePlaymodeChange(PlayModeStateChange obj)
	{
		switch (obj)
		{
			case PlayModeStateChange.EnteredEditMode:
				break;

			case PlayModeStateChange.ExitingEditMode:
				break;

			case PlayModeStateChange.EnteredPlayMode:
				selectedTurnIndex = -1;
				EditorApplication.update -= RepaintWindow;
				EditorApplication.update += RepaintWindow;
				break;

			case PlayModeStateChange.ExitingPlayMode:
				EditorApplication.update -= RepaintWindow;
				break;

			default:
				break;
		}
	}

	private void RepaintWindow() => Repaint();

	private void OnFocus()
	{
		playIcon = EditorGUIUtility.IconContent("PlayButton On");
		undoIcon = EditorGUIUtility.IconContent("d_TreeEditor.Refresh");
		ffIcon = EditorGUIUtility.IconContent("d_StepButton On");
	}

	private void OnGUI()
	{
		if (!Application.isPlaying)
		{
			//debugScrollPos = EditorGUILayout.BeginScrollView(debugScrollPos);
			//EditorGUILayout.LabelField("Not drawing history outside of playmode, yet...");
			DrawDebugPicker();
			//EditorGUILayout.EndScrollView();
			return;
		}

		if (Haxan.stateVariable == null || Haxan.stateVariable.state.history == null)
		{
			EditorGUILayout.LabelField("No history found...");
			return;
		}

		TickInputs();

		EditorGUILayout.BeginHorizontal();
		DrawRuntimeControls();
		DrawSelectedTurn();
		EditorGUILayout.EndHorizontal();
		
		return;

		var viewWidth = EditorGUIUtility.currentViewWidth;
		EditorGUILayout.BeginHorizontal();
		{
			using(new EditorGUILayout.VerticalScope("box", GUILayout.MaxWidth(200f), GUILayout.ExpandHeight(true)))
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					DrawSelectionGrid();
					DrawTurnControls();
					//DrawSelectionGrid();
				}

				//using (new EditorGUILayout.HorizontalScope())
				
				//DrawTurnControls();
				//using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(150f), GUILayout.ExpandHeight(true)))
				//{
				//}

				//using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(50f), GUILayout.ExpandHeight(true)))
				//{
				//}
			}

			//EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150f), GUILayout.ExpandHeight(true));
			//{
				
			//}
			//EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
			{
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
				{
					if (selectedTurnIndex < 0)
					{
						EditorGUILayout.LabelField("Select a turn to display its data.");
					}
					else
					{
						Turn turn = Haxan.history.turns[selectedTurnIndex];
						if (turn.instigator != null)
							EditorGUILayout.ObjectField(turn.instigator, typeof(Unit), true);
						else
							EditorGUILayout.LabelField("... instigator was null tho");

						EditorGUILayout.LabelField($"START TIME: {turn.startTime}");
						EditorGUILayout.LabelField($"END TIME: {turn.endTime}");
						EditorGUILayout.LabelField($"DURATION: {turn.duration}");

						GUILayout.Space(20);
						using (new GUILayout.VerticalScope(EditorStyles.helpBox))
						{
							for (int i = turn.stepIndex; i < turn.stepIndex + turn.stepCount; i++)
							{
								TurnStep turnStep = Haxan.history.turnSteps[i];
								for (int j = turnStep.opIndex; j < turnStep.opIndex + turnStep.opCount; j++)
								{
									UnitOp op = Haxan.history.allOps[j];
									if (op == null)
										continue;
									op.DrawInspectorContent();
								}
							}
						}
					}
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();
	}


	private void TickInputs()
	{
		var e = Event.current;
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.DownArrow)
		{
			Debug.LogWarning("DOWN:");
			if (selectedTurnIndex < Haxan.history.turnCount)
			{
				selectedTurnIndex++;
				Repaint();
				return;
			}
		}

		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.UpArrow)
		{
			Debug.LogWarning("UP:");
			if (selectedTurnIndex > 0)
			{
				selectedTurnIndex--;
				Repaint();
				return;
			}
		}
	}

	private void DrawRuntimeControls()
	{
		float buttonWidth = 24f;
		float nameWidth = 80f;
		float extraPadding = 40f;


		using (new EditorGUILayout.VerticalScope(
			"box",
			GUILayout.Width(200f + extraPadding),
			GUILayout.ExpandHeight(true)
			))
		{
			if (Haxan.stateVariable.state.history.turnCount == 0)
			{
				EditorGUILayout.LabelField(
					"No turns in history.", 
					EditorStyles.boldLabel
					);
				return;
			}

			debugScrollPos = EditorGUILayout.BeginScrollView(debugScrollPos);

			//... unnecessary "START" button.. ?

			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button(playIcon, GUILayout.Width(buttonWidth)))
				{
					//Debug.LogWarning("HIT PLAY ON TURN : " + i);
					GameContext.OnScrubToTurnClicked?.Invoke(-1);
				}

				if (GUILayout.Button(ffIcon, GUILayout.Width(buttonWidth)))
				{
					//Debug.LogWarning("HIT FF ON TURN : " + i);
					GameContext.OnSmashToTurnClicked?.Invoke(-1);
				}

				if (GUILayout.Button("START", GUILayout.Width(nameWidth)))
				{
					//Debug.LogWarning($"Displaying turn {i}");
					selectedTurnIndex = -1;
				}
			}

			for (int i = 0; i < Haxan.stateVariable.state.history.turnCount; i++)
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					int buttonCount = 3;
					if (GUILayout.Button(undoIcon, GUILayout.Width(buttonWidth)))
					{
						//Debug.LogWarning("HIT PLAY ON TURN : " + i);
						GameContext.OnScrubToTurnClicked?.Invoke(i);
					}

					if (GUILayout.Button(playIcon, GUILayout.Width(buttonWidth)))
					{
						//Debug.LogWarning("HIT PLAY ON TURN : " + i);
						GameContext.OnScrubToTurnClicked?.Invoke(i + 1);
					}

					if (GUILayout.Button(ffIcon, GUILayout.Width(buttonWidth)))
					{
						//Debug.LogWarning("HIT FF ON TURN : " + i);
						GameContext.OnSmashToTurnClicked?.Invoke(i);
					}

					if (GUILayout.Button("SHOW", GUILayout.Width(nameWidth)))
					{
						//Debug.LogWarning($"Displaying turn {i}");
						selectedTurnIndex = i;
					}

					Rect rect = GUILayoutUtility.GetRect(
						200 - buttonWidth * buttonCount - nameWidth,
						EditorGUIUtility.singleLineHeight
						);

					float bump = 2f;
					rect.yMin += bump;
					rect.yMin += 1f;
					rect.yMax += bump;
					EditorGUI.DrawRect(rect, ColorPicker.Swatches.collisionProbe);

					Rect overRect = rect;
					var width = overRect.width;


					overRect.width = width * currSliderValue;
					EditorGUI.DrawRect(overRect, ColorPicker.Swatches.trigger);

					float markerWidth = 2f;
					float markerHeightBump = 2f;
					Rect markerRect = rect;

					markerRect.xMin = overRect.xMax - markerWidth;
					markerRect.xMax = overRect.xMax - markerWidth;
					markerRect.height += markerHeightBump;
				}
			}
			EditorGUILayout.EndScrollView();
		}
	}

	private void DrawSelectedTurn()
	{
		if(selectedTurnIndex < 0)
		{
			using (new EditorGUILayout.VerticalScope("box", GUILayout.ExpandHeight(true)))
			{
				EditorGUILayout.LabelField("Select a turn to show its data.", EditorStyles.boldLabel);
			}
			return;
		}

		EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			{
				if (selectedTurnIndex < 0)
				{
					EditorGUILayout.LabelField("Select a turn to display its data.");
				}
				else
				{
					EditorGUILayout.LabelField($"SHOWING TURN: {selectedTurnIndex}");

					Turn turn = Haxan.history.turns[selectedTurnIndex];
					if (turn.instigator != null)
						EditorGUILayout.ObjectField(turn.instigator, typeof(Unit), true);
					else
						EditorGUILayout.LabelField("... instigator was null tho");

					EditorGUILayout.LabelField($"START TIME: {turn.startTime}");
					EditorGUILayout.LabelField($"END TIME: {turn.endTime}");
					EditorGUILayout.LabelField($"DURATION: {turn.duration}");

					GUILayout.Space(20);
					for (int i = turn.stepIndex; i < turn.stepIndex + turn.stepCount; i++)
					{
						using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
						{
							TurnStep turnStep = Haxan.history.turnSteps[i];
							for (int j = turnStep.opIndex; j < turnStep.opIndex + turnStep.opCount; j++)
							{
								UnitOp op = Haxan.history.allOps[j];
								if (op == null)
									continue;
								op.DrawInspectorContent();
							}
						}
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}
		EditorGUILayout.EndVertical();
	}

	private void DrawTurnControls()
	{
		GUIContent playIcon = EditorGUIUtility.IconContent("d_PlayButton On");
		for (int i = 0; i < Haxan.history.turnCount; i++)
		{
			if(GUILayout.Button(playIcon, GUILayout.Width(24)))
			{
				Debug.LogWarning("HIT PLAY ON TURN : " + i);
			}
		}
	}

	private void DrawSelectionGrid()
	{
		var buttonNames = new string[Haxan.history.turnCount];
		for (int i = 0; i < Haxan.history.turnCount; i++)
		{
			var instigatorName = Haxan.history.turns[i].instigator.name.ToUpper();
			//var instigatorName = "instigator";
			//if(selectedTurnIndex < Haxan.history.turnCount)
			//{
			//	var instigator = Haxan.history.turns[selectedTurnIndex].instigator;
			//	if (instigator != null)
			//		instigatorName = instigator.name.ToUpper();
			//}
			buttonNames[i] = $"TURN {i} // {instigatorName}";
		}
		selectedTurnIndex = GUILayout.SelectionGrid(selectedTurnIndex, buttonNames, 1);
	}



	private void DrawDebugPicker()
	{
		GUIContent playIcon = EditorGUIUtility.IconContent("PlayButton On");
		GUIContent ffIcon = EditorGUIUtility.IconContent("d_StepButton On");

		float buttonWidth = 24f;
		float nameWidth = 80f;
		float extraPadding = 40f;

		EditorGUI.BeginChangeCheck();
		currSliderValue = EditorGUILayout.Slider(currSliderValue, 0f, 1f);
		bool changed = EditorGUI.EndChangeCheck();


		EditorGUILayout.BeginHorizontal();
		using (new EditorGUILayout.VerticalScope(
			"box",
			GUILayout.Width(200f + extraPadding),
			//GUILayout.MaxWidth(200f + extraPadding),
			GUILayout.ExpandHeight(true)
			//,
			//GUILayout.ExpandWidth(false)
			))
		{
			debugScrollPos = EditorGUILayout.BeginScrollView(debugScrollPos);
			for (int i = 0; i < debugTurnCount; i++)
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					if (GUILayout.Button(playIcon, GUILayout.Width(buttonWidth)))
					{
						Debug.LogWarning("HIT PLAY ON TURN : " + i);
					}

					if (GUILayout.Button(ffIcon, GUILayout.Width(buttonWidth)))
					{
						Debug.LogWarning("HIT FF ON TURN : " + i);
					}

					if (GUILayout.Button("TURN NAME", GUILayout.Width(nameWidth)))
					{

					}

					//Rect lastRect = GUILayoutUtility.GetLastRect();
					//Rect rect = lastRect;
					//float drawn = buttonWidth * 2f + nameWidth;
					//rect.width = 200 - drawn;
					//rect.xMin += drawn;

					Rect rect = GUILayoutUtility.GetRect(
						//GUIContent.none, 
						200 - buttonWidth * 2f - nameWidth,
						EditorGUIUtility.singleLineHeight
						//,
						//GUILayout.ExpandWidth(true)
						);
					float bump = 2f;
					rect.yMin += bump;
					rect.yMin += 1f;
					rect.yMax += bump;// + 1f;

					//Rect rect = GUILayoutUtility.GetLastRect();
					//Rect rect = EditorGUILayout.GetControlRect();

					//rect.width *= currSliderValue;
					//rect.width = rect.width - 24 - 24 - 80;
					EditorGUI.DrawRect(rect, ColorPicker.Swatches.collisionProbe);

					Rect overRect = rect;
					var width = overRect.width;
					overRect.width = width * currSliderValue;
					EditorGUI.DrawRect(overRect, ColorPicker.Swatches.trigger);

					float markerWidth = 2f;
					float markerHeightBump = 2f;
					Rect markerRect = rect;
					//markerRect.width = markerWidth;

					markerRect.xMin = overRect.xMax - markerWidth;
					markerRect.xMax = overRect.xMax - markerWidth;
					markerRect.height += markerHeightBump;
					//EditorGUI.DrawRect(markerRect, Color.white);

					//overRect.xMin += overRect.width;
					//GUILayout.Box(
					//	EditorGUIUtility.whiteTexture
					//	//GUILayout.ExpandHeight(true), 
					//	//GUILayout.ExpandWidth(true)
					//	);
				}
			}
			EditorGUILayout.EndScrollView();
		}

		using (new EditorGUILayout.VerticalScope("box", GUILayout.ExpandHeight(true)))
		{
			EditorGUILayout.LabelField("NEXT SECTION:");
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.EndHorizontal();
	}

	private void DrawDebugSelectedTurn()
	{
		using (new EditorGUILayout.VerticalScope("box", GUILayout.ExpandHeight(true)))
		{
			EditorGUILayout.LabelField("NEXT SECTION:");
		}
	}
}
