using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
	private void OnEnable()
	{
		SaveManager.OnBoardStateLoadStart += OnLoadStart;
		SaveManager.OnBoardStateLoadComplete += OnLoadComplete;
	}

	private void OnDisable()
	{
		SaveManager.OnBoardStateSaveStart -= OnLoadStart;
		SaveManager.OnBoardStateLoadComplete -= OnLoadComplete;
	}

	private void OnLoadStart(BoardState newBoardState)
	{
		Debug.LogWarning("Reloading board units.");

		var allUnits = new List<Unit>();

		foreach (var unit in GameSystems.activeUnits.Items)
			allUnits.Add(unit);

		foreach (var unit in allUnits)
		{
			Destroy(unit.gameObject);
		}
	}

	private void OnLoadComplete(BoardState newBoardState)
	{
		Debug.LogWarning("loading new units: " + newBoardState.unitStates.Count);

		foreach (var unitState in newBoardState.unitStates)
		{
			var unitPrefab = Resources.Load("Prefabs/Units/Cedric") as GameObject;
			var unitInstance = Instantiate(unitPrefab).GetComponent<Unit>();
			unitInstance.SetState(unitState);
		}
	}
}
