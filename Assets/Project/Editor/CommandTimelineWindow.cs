using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CommandTimelineWindow : EditorWindow
{
	[MenuItem("Haxan/CommandTimeline")]
	public static void ShowWindow()
	{
		// Get existing open window or if none, make a new one:
		CommandTimelineWindow window = (CommandTimelineWindow)EditorWindow.GetWindow(typeof(CommandTimelineWindow));
		window.Show();
	}

	CommandTimeline timeline;
	Texture2D commandBackground;
	GUIStyle commandStyle;
	private void OnEnable()
	{
		timeline = FindObjectOfType<CommandTimeline>();
		if (timeline == null)
			return;

		commandBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
		commandBackground.SetPixel(0, 0, timeline.commandColor);
		commandBackground.Apply(); // not sure if this is necessary

		// basically just create a copy of the "none style"
		// and then change the properties as desired
		commandStyle = new GUIStyle(EditorStyles.helpBox);
		commandStyle.fontSize = timeline.commandTextSize;
		commandStyle.fontStyle = FontStyle.Bold;
		commandStyle.normal.textColor = Color.white;
		commandStyle.normal.background = commandBackground;

		EditorApplication.update += Repaint;
	}

	private void OnDisable() => EditorApplication.update -= Repaint;

	void OnFocus()
	{
		if (timeline == null)
			timeline = FindObjectOfType<CommandTimeline>();
	}

	Vector2 scrollPos;
	private void OnGUI()
	{
		if (timeline == null)
		{
			GUILayout.Label("... no timeline found.", EditorStyles.boldLabel);
			if (GUILayout.Button("CREATE TIMELINE"))
			{
				GameObject newObj = new GameObject("Command Timeline");
				timeline = newObj.AddComponent<CommandTimeline>();
				EditorUtility.SetDirty(newObj);
			}
			return;
		}

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);//, GUILayout.Width(100), GUILayout.Height(100));
		float marginSize = EditorStyles.helpBox.margin.right;
		using (new GUILayout.HorizontalScope(/*EditorStyles.helpBox*/))
		{
			for (int i = 0; i < timeline.fakeInstigatingCommands.Count; i++)
			{
				using (new GUILayout.HorizontalScope(commandStyle, GUILayout.Width(timeline.itemWidth)))
				//using (new GUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.Width(timeline.itemWidth)))
				{ 
					GUILayout.Label(
						timeline.fakeInstigatingCommands[i].name.ToUpper(),
						commandStyle,
						GUILayout.Height(timeline.itemHeight)
						//, GUILayout.Width(timeline.itemWidth)
						);
				}
			}
		}
		EditorGUILayout.EndScrollView();
	}
}
