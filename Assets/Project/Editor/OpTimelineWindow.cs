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

	private void OnGUI()
	{
		if (boardUI == null)
			return;

		for (int i = 0; i < boardUI.numDummyOps; i++)
		{
			Rect rect = EditorGUILayout.GetControlRect();
			EditorGUI.DrawRect(rect, ColorPicker.Swatches.collisionProbe);
			//GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
		}
	}
}
