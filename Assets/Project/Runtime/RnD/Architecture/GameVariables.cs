using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameVariables
{
	private const string activeUnitsPath = "RuntimeSets/ActiveUnits";
	public static UnitRuntimeSet activeUnits { get; private set; }

	private const string boardStatePath = "GameFlow/BoardState";
	public static BoardStateVariable board { get; private set; }

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
		board = Resources.Load(boardStatePath, typeof(BoardStateVariable)) as BoardStateVariable;

		var allUnitArchetypes = Resources.LoadAll<UnitDefinition>("UnitArchetypes").ToList();
		allUnitArchetypes.ToDictionary(r => r.type, r => r);
	}
}