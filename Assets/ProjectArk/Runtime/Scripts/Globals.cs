using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public interface IBootable
{
    void Bootup();
}

public static class Globals 
{
	private const string wanderersPath = "RuntimeSets/ActiveWanderers";
	private const string selectedWandererPath = "RuntimeSets/SelectedWanderer";
	private const string deadWanderersPath = "RuntimeSets/DeadWanderers";
	private const string enemiesPath = "RuntimeSets/ActiveEnemies";
	private const string deadEnemiesPath = "RuntimeSets/DeadEnemies";
	private const string readyWanderersPath = "RuntimeSets/ReadyWanderers";
	private const string readyEnemiesPath = "RuntimeSets/ReadyEnemies";

	public static CharacterRuntimeSet ActiveWanderers { get; private set; }
	public static CharacterRuntimeSet SelectedWanderer { get; private set; }
	public static CharacterRuntimeSet DeadWanderers { get; private set; }
	public static CharacterRuntimeSet ActiveEnemies { get; private set; }
	public static CharacterRuntimeSet DeadEnemies { get; private set; }
	public static CharacterRuntimeSet ReadyWanderers { get; private set; }
	public static CharacterRuntimeSet ReadyEnemies { get; private set; }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Bootup()
    {
        //Debug.Log("Globals up & running");

        //var allBootables = InterfaceHelper.FindObjects<IBootable>();

        //Debug.Log("... found " + allBootables.Count + " bootables.");

        //foreach (var bootable in allBootables)
        //    bootable.Bootup();

        //var allBootables = InterfaceHelper.GetInterfaceComponents<IBootable>();
        //var allBootables = Object.FindObjectOfType<IBootable>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void LoadRuntimeAssets()
	{
        ActiveWanderers = Resources.Load(
			wanderersPath,
			typeof(CharacterRuntimeSet)
			) as CharacterRuntimeSet;

		SelectedWanderer = Resources.Load(
			selectedWandererPath,
			typeof(CharacterRuntimeSet)
			) as CharacterRuntimeSet;

		//DeadWanderers = Resources.Load(
		//	deadWanderersPath, 
		//	typeof(CharacterRuntimeSet)) as CharacterRuntimeSet;

		ActiveEnemies = Resources.Load(
			enemiesPath,
			typeof(CharacterRuntimeSet)
			) as CharacterRuntimeSet;

		//DeadEnemies = Resources.Load(
		//	deadEnemiesPath, 
		//	typeof(CharacterRuntimeSet)) as CharacterRuntimeSet;

		ReadyWanderers = Resources.Load(
			readyWanderersPath,
			typeof(CharacterRuntimeSet)
			) as CharacterRuntimeSet;

		ReadyEnemies = Resources.Load(
			readyEnemiesPath,
			typeof(CharacterRuntimeSet)
			) as CharacterRuntimeSet;
	}

	private static Camera camera;
	public static Camera Camera
	{
		get
		{
			if (camera == null)
				camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
				//camera = GameObject.FindObjectOfType<Camera>();
			return camera;
		}
	}

	private static EventSystem eventSystem;
	public static EventSystem EventSystem
	{
		get
		{
			if (eventSystem == null)
				eventSystem = GameObject.FindObjectOfType<EventSystem>();
			return eventSystem;
		}
	}

	private static HexGrid grid;
	public static HexGrid Grid
	{
		get
		{
			if (grid == null)
				grid = GameObject.FindObjectOfType<HexGrid>();
			return grid;
		}
	}

	private static MainFlowController mainFlow;
	public static MainFlowController MainFlow
	{
		get
		{
			if (mainFlow == null)
				mainFlow = GameObject.FindObjectOfType<MainFlowController>();
			return mainFlow;
		}
	}

	//private static TimelineManager timelines;
	//public static TimelineManager Timelines
	//{
	//	get
	//	{
	//		if (timelines == null)
	//			timelines = GameObject.FindObjectOfType<TimelineManager>();
	//		return timelines;
	//	}
	//}

	//private static Wanderer player;
	//public static Wanderer Player
	//{
	//	get
	//	{
	//		if (player == null)
	//			player = GameObject.FindObjectOfType<Wanderer>();
	//		return player;
	//	}
	//}

	//private static Gamestate gameState;
	//public static Gamestate GameState
	//{
	//	get
	//	{
	//		if (gameState == null)
	//			gameState = GameObject.FindObjectOfType<Gamestate>();
	//		return gameState;
	//	}
	//}

	//private static HexGrid hexGrid;
	//public static HexGrid HexGrid
	//{
	//	get
	//	{
	//		if(hexGrid == null)
	//			hexGrid = GameObject.FindObjectOfType<HexGrid>();
	//		return hexGrid;
	//	}
	//}

	//private static HexGridSelector selector;
	//public static HexGridSelector Selector
	//{
	//	get
	//	{
	//		if (selector == null)
	//			selector = GameObject.FindObjectOfType<HexGridSelector>();
	//		return selector;
	//	}
	//}

	//private static Pathfinder pathfinder;
	//public static Pathfinder Pathfinder
	//{
	//	get
	//	{
	//		if (pathfinder == null)
	//			pathfinder = GameObject.FindObjectOfType<Pathfinder>();
	//		return pathfinder;
	//	}
	//}

	//private static TimeManager timeManager;
	//public static TimeManager TimeManager
	//{
	//	get
	//	{
	//		if (timeManager == null)
	//			timeManager = GameObject.FindObjectOfType<TimeManager>();
	//		return timeManager;
	//	}
	//}

	//private static ScreenManager screenManager;
	//public static ScreenManager ScreenManager
	//{
	//	get
	//	{
	//		if (screenManager == null)
	//			screenManager = GameObject.FindObjectOfType<ScreenManager>();
	//		return screenManager;
	//	}
	//}

	//private static LevelManager levelManager;
	//public static LevelManager LevelManager
	//{
	//	get
	//	{
	//		if (levelManager == null)
	//			levelManager = GameObject.FindObjectOfType<LevelManager>();
	//		return levelManager;
	//	}
	//}

	//private static MatchFlowController matchFlow;
	//public static MatchFlowController MatchFlow
	//{
	//	get
	//	{
	//		if (matchFlow == null)
	//			matchFlow = Object.FindObjectOfType<MatchFlowController>();
	//		return matchFlow;
	//	}
	//}

	//private static SpawnManager spawnManager;
	//public static SpawnManager SpawnManager
	//{
	//	get
	//	{
	//		if (spawnManager == null)
	//			spawnManager = GameObject.FindObjectOfType<SpawnManager>();
	//		return spawnManager;
	//	}
	//}

	//private static TurnFlowManager turnFlow;
	//public static TurnFlowManager TurnFlow
	//{
	//	get
	//	{
	//		if (turnFlow == null)
	//			turnFlow = GameObject.FindObjectOfType<TurnFlowManager>();
	//		return turnFlow;
	//	}
	//}
}
