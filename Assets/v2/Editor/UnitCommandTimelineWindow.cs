using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitCommandTimelineWindow : EditorWindow
{
    [MenuItem("Tools/UnitCommandTimeline")]
    public static void ShowWindow()
	{
		// Get existing open window or if none, make a new one:
		UnitCommandTimelineWindow window = (UnitCommandTimelineWindow)EditorWindow.GetWindow(typeof(UnitCommandTimelineWindow));
		window.Show();
	}

	UnitCommandTimeline timeline;

	private void OnEnable()
	{
		//Debug.LogWarning("ENABLED WINDOW");
		timeline = FindObjectOfType<UnitCommandTimeline>();
	}

	private void OnDisable()
	{
		//Debug.LogWarning("DISABLED WINDOW");
	}

	void OnFocus()
	{
		if(timeline == null)
			timeline = FindObjectOfType<UnitCommandTimeline>();
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
				timeline = newObj.AddComponent<UnitCommandTimeline>();
				EditorUtility.SetDirty(newObj);
			}
			return;
		}

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);//, GUILayout.Width(100), GUILayout.Height(100));
		using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
		{
			for (int i = 0; i < timeline.length; i++)
			{
				GUILayout.Label(
					i.ToString(), EditorStyles.helpBox, 
					GUILayout.Height(timeline.itemHeight), GUILayout.Width(timeline.itemWidth)
					);
				//, GUILayout.ExpandHeight(true));
				//GUILayout.Label(i.ToString(), EditorStyles.)
				//GUILayout.Button(i.ToString());
			}

			//using (new GUILayout.VerticalScope(EditorStyles.helpBox))
			//{
			//	EditorGUILayout.BeginHorizontal();
			//EditorGUILayout.EndHorizontal();
			//}
		}
		EditorGUILayout.EndScrollView();
	}

	//// Start is called before the first frame update
	//void Start()
	//{

	//}

	//// Update is called once per frame
	//void Update()
	//{

	//}
}
