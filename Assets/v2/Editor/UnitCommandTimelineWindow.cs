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

	private void OnGUI()
	{
		if(timeline == null)
		{
			GUILayout.Label("... no timeline found.", EditorStyles.boldLabel);
			if(GUILayout.Button("CREATE TIMELINE"))
			{
				GameObject newObj = new GameObject("Command Timeline");
				timeline = newObj.AddComponent<UnitCommandTimeline>();
				EditorUtility.SetDirty(newObj);
			}
			return;
		}

		EditorGUILayout.BeginHorizontal();
		for (int i = 0; i < timeline.length; i++)
		{
			GUILayout.Button(i.ToString());
		}
		EditorGUILayout.EndHorizontal();
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
