using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class BoardManager
{
    static BoardManager()
    {
        EditorSceneManager.sceneOpened += SceneOpenedCallback;
        EditorSceneManager.sceneSaving += SceneSavingCallback;
		EditorSceneManager.sceneSaved += SceneSavedCallback;
		EditorSceneManager.sceneDirtied += SceneDirtiedCallback;
		//EditorSceneManager.sceneSaving
	}

	private static void SceneDirtiedCallback(Scene scene)
	{
		Debug.LogWarning("Scene dirtied...");
		
	}

	private static void SceneSavingCallback(Scene scene, string path)
	{
		Debug.LogWarning("Scene saving...");
		RefreshBoardData();
	}

	private static void SceneSavedCallback(Scene scene)
	{
		Debug.LogWarning("... scene saved.");
		//RefreshBoardData();
	}



	//private static void SceneSavedCallback(Scene scene)
	//{

	//}

	private static void SceneOpenedCallback(Scene _scene, OpenSceneMode _mode)
    {
		RefreshBoardData();
    }

	public static void RefreshBoardData()
	{
		GridV2 foundGrid = GameObject.FindObjectOfType<GridV2>();
		if (foundGrid == null)
			return;

		BoardData boardData = GameObject.FindObjectOfType<BoardData>();
		if (boardData == null)
		{
			Debug.LogWarning("No board data found, creating some.");
			GameObject newBoardDataObject = new GameObject("Board Data");
			boardData = newBoardDataObject.AddComponent<BoardData>();
		}

		boardData.Refresh();
	}
}
