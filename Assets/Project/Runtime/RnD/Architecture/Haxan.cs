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
	
	private const string boardStateVariablePath = "GameFlow/BoardStateVariable";

	public static BoardStateVariable stateVariable { get; set; }

	public static BoardState state { get => stateVariable.state; set => stateVariable.state = value; }

	public static BoardHistory history => stateVariable.state.history;

	public static BoardLayout layout => stateVariable.state.layout;

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
		stateVariable = Resources.Load(boardStateVariablePath, typeof(BoardStateVariable)) as BoardStateVariable;

		Haxan.history.turnCount = 0;
		Haxan.history.totalCreatedTurnSteps = 0;
		Haxan.history.totalCreatedOps = 0;
		Haxan.history.currPlaybackTurn = 0;

		//state.history.turnCount = 0;

		var allUnitArchetypes = Resources.LoadAll<UnitDefinition>("UnitArchetypes").ToList();
		allUnitArchetypes.ToDictionary(r => r.type, r => r);
	}
}