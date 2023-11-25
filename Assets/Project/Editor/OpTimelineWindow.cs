using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OpTimelineWindow : EditorWindow
{
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

	//BoardUI boardUI;
	//Texture backgroundTexture;
	//Texture opTexture;
	private void OnEnable()
	{
		//boardUI = FindObjectOfType<BoardUI>();
		//opTexture = Texture;
	}

	private void OnFocus()
	{
		//if (boardUI == null)
		//	boardUI = FindObjectOfType<BoardUI>();
	}

	int selectedTurnIndex = -1;
	//private const int numTurns = 5;
	Vector2 scrollPos;
	private void OnGUI()
	{
		if (!Application.isPlaying)
		{
			//EditorGUILayout.LabelField("Not drawing history outside of playmode, yet...");
			DrawDebugPicker();
			return;
		}

		if (Haxan.state == null || Haxan.state.history == null)
		{
			EditorGUILayout.LabelField("No history found...");
			return;
		}

		var e = Event.current;

		if(e.type == EventType.KeyDown && e.keyCode == KeyCode.DownArrow)
		{
			Debug.LogWarning("DOWN:");
			if(selectedTurnIndex < Haxan.history.turnCount)
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
									IUnitOperable op = Haxan.history.allOps[j];
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

	//[Header()]
	private const int debugTurnCount = 5;
	public float currSliderValue;
	public float currFloatSliderValue;
	private void DrawDebugPicker()
	{
		GUIContent playIcon = EditorGUIUtility.IconContent("PlayButton On");
		GUIContent ffIcon = EditorGUIUtility.IconContent("d_StepButton On");

		currSliderValue = EditorGUILayout.Slider(currSliderValue, 0f, 1f);

		EditorGUILayout.BeginHorizontal();
		using (new EditorGUILayout.VerticalScope("box", GUILayout.MaxWidth(200f), GUILayout.ExpandHeight(true)))
		{
			//GUILayout
			for (int i = 0; i < debugTurnCount; i++)
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					if (GUILayout.Button(playIcon, GUILayout.Width(24)))
					{
						Debug.LogWarning("HIT PLAY ON TURN : " + i);
					}

					if (GUILayout.Button(ffIcon, GUILayout.Width(24)))
					{
						Debug.LogWarning("HIT PLAY ON TURN : " + i);
					}

					//EditorStyles.
					if (GUILayout.Button("TURN NAME"))
					{

					}

					Rect rect = EditorGUILayout.GetControlRect();
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
					EditorGUI.DrawRect(markerRect, Color.white);

					//overRect.xMin += overRect.width;

					//GUILayout.Box(
					//	EditorGUIUtility.whiteTexture
					//	//GUILayout.ExpandHeight(true), 
					//	//GUILayout.ExpandWidth(true)
					//	);
				}
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
		EditorGUILayout.EndHorizontal();
		//EditorGUILayout.BeginHorizontal();
		//using (new EditorGUILayout.VerticalScope("box", GUILayout.MaxWidth(200f), GUILayout.ExpandHeight(true)))
		//{
			//currSliderValue = EditorGUILayout.IntSlider(currSliderValue, 0, debugTurnCount);
			//currFloatSliderValue = GUILayout.VerticalSlider(currFloatSliderValue, 0f, debugTurnCount);
		//}
		//EditorGUILayout.EndHorizontal();
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

	void DrawSideBarContent()
	{
		if (GUILayout.Button("CLEAR"))
			selectedTurnIndex = -1;

		for (int i = 0; i < Haxan.history.turnCount; i++)
		{
			if(GUILayout.Button($"TURN {i}"))
			{
				selectedTurnIndex = i;
			}	
		}
	}

	void DrawOpTimelines()
	{
		//for (int i = 0; i < boardUI.numDummyOps; i++)
		//{
		//	Rect rect = EditorGUILayout.GetControlRect();
		//	EditorGUI.DrawRect(rect, ColorPicker.Swatches.collisionProbe);
		//	//GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
		//}
	}
}
