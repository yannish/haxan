using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IPreloadable
{
	void Preload();
}

public class Spine : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Preload()
	{
		Spine foundSpine = FindObjectOfType<Spine>();
		if (!foundSpine)
			return;

		DontDestroyOnLoad(foundSpine.gameObject);

		var preloadables = foundSpine.GetComponentsInChildren<IPreloadable>();
		foreach (var preloadable in preloadables)
		{
			preloadable.Preload();
		}
	}
}

[InitializeOnLoad]
public class Bootup
{
	static Bootup()
	{
		//Debug.Log("BOOTING UP");
		//EditorSceneManager.sceneOpened += OnSceneLoaded;
	}

	static void OnSceneLoaded(Scene scene, OpenSceneMode mode)
	{
		//Debug.Log("SCENE LOADED");
		//      GridActions.RefreshCellBindings();

		//Debug.Log("REFRESHING CELL BINDINGS");

		//if (Globals.Grid)
		//{

		//}

		//AssemblyReloadEvents.afterAssemblyReload += RefreshCellBindings;
	}

	static void RefreshCellBindings()
	{

	}
}