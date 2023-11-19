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

	BoardUI boardUI;
	//Texture backgroundTexture;
	//Texture opTexture;
	private void OnEnable()
	{
		boardUI = FindObjectOfType<BoardUI>();
		//opTexture = Texture;
	}

	private void OnFocus()
	{
		if (boardUI == null)
			boardUI = FindObjectOfType<BoardUI>();
	}

	int selectedTurnIndex = -1;
	private const int numTurns = 5;
	private void OnGUI()
	{
		if (boardUI == null)
			return;

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150f), GUILayout.ExpandHeight(true));
			DrawSideBarContent();
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
			if(selectedTurnIndex < 0)
			{
				EditorGUILayout.LabelField("Select a turn to display its data.");
			}
			else
			{

			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();
	}

	void DrawSideBarContent()
	{
		if (GUILayout.Button("CLEAR"))
			selectedTurnIndex = -1;

		for (int i = 0; i < numTurns; i++)
		{
			if(GUILayout.Button($"TURN {i}"))
			{
				selectedTurnIndex = i;
			}	
		}
	}

	void DrawOpTimelines()
	{
		for (int i = 0; i < boardUI.numDummyOps; i++)
		{
			Rect rect = EditorGUILayout.GetControlRect();
			EditorGUI.DrawRect(rect, ColorPicker.Swatches.collisionProbe);
			//GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
		}
	}
}
