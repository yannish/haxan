using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
	private void OnEnable()
	{
		GameContext.OnLoadBoardStateBegin += ClearBoard;
		GameContext.OnLoadBoardStateComplete += SetBoard;
	}

	private void OnDisable()
	{
		GameContext.OnLoadBoardStateBegin -= ClearBoard;
		GameContext.OnLoadBoardStateComplete -= SetBoard;
	}

	private void ClearBoard()
	{
		Debug.LogWarning("Clearing board units.");

		for (int i = Haxan.activeUnits.Items.Count - 1; i >= 0; i--)
		{
			var unit = Haxan.activeUnits.Items[i];
			Destroy(unit.gameObject);
		}
	}

	private void SetBoard()
	{
		Debug.LogWarning("... loading new units from save: " + Haxan.state.layout.unitStates.Count);

		foreach (var unitState in Haxan.state.layout.unitStates)
		{
			var unitPrefab = Resources.Load(unitState.templatePath) as GameObject;
			var unitInstance = Instantiate(unitPrefab).GetComponent<Unit>();
			unitInstance.SetState(unitState);
		}
	}
}
