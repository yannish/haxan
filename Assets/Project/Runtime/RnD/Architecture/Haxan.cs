using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Haxan
{
	private const string activeUnitsPath = "RuntimeSets/ActiveUnits";
	public static UnitRuntimeSet activeUnits { get; private set; }
	public static List<Unit> units => activeUnits.Items;

	private const string boardStatePath = "GameFlow/BoardState";

	public static BoardState state { get; set; }

	public static BoardHistory history => state.history;

	public static BoardLayout layout => state.layout;

	public static Unit ToUnit(this int index) => activeUnits.Items[index];

	public static int ToIndex(this Unit unit)
	{
		for (int i = 0; i < activeUnits.Items.Count; i++)
		{
			if (activeUnits.Items[i] == unit)
				return i;
		}

		return -1;
	}

	//... should i set a look-up from index to unit..?

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void LoadRuntimeResources()
	{
		Debug.LogWarning("LOADED RUNTIME VARIABLES");

		activeUnits = Resources.Load(activeUnitsPath, typeof(UnitRuntimeSet)) as UnitRuntimeSet;
		state = Resources.Load(boardStatePath, typeof(BoardState)) as BoardState;

		state.history.turnCount = 0;
		state.history.totalCreatedTurnSteps = 0;
		state.history.totalCreatedOps = 0;

		var allUnitArchetypes = Resources.LoadAll<UnitDefinition>("UnitArchetypes").ToList();
		allUnitArchetypes.ToDictionary(r => r.type, r => r);
	}
}