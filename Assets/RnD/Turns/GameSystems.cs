using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystems : MonoBehaviour
{
	private const string activeUnitsPath = "RuntimeSets/ActiveUnits";
	public static UnitRuntimeSet activeUnits { get; private set; }

	public SaveManager saveManager;


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
	{
		var contextInstanceObj = Instantiate(Resources.Load("Systems")) as GameObject;
		I = contextInstanceObj.GetComponent<GameSystems>();
		DontDestroyOnLoad(contextInstanceObj);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void LoadRuntimeResources()
	{
		activeUnits = Resources.Load(activeUnitsPath, typeof(UnitRuntimeSet)) as UnitRuntimeSet;
	}

	public static GameSystems I;
	private void Awake()
	{
		if(I != null)
		{
			Destroy(this.gameObject);
			return;
		}

		I = this;
		DontDestroyOnLoad(this.gameObject);

		//FetchResources();
	}
}
