using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
	const string boardStatePathName = "/BoardState.json";

	public BoardState boardState = new BoardState();


	private void OnEnable()
	{
		GameContext.OnSaveBoardState += SaveBoardState;
		GameContext.OnLoadBoardStte += LoadBoardState;
	}

	private void OnDisable()
	{
		GameContext.OnSaveBoardState -= SaveBoardState;
		GameContext.OnLoadBoardStte -= LoadBoardState;
	}


	private void SaveBoardState()
	{
		GameVariables.board.unitStates.Clear();
		foreach (var unit in GameVariables.activeUnits.Items)
		{
			GameVariables.board.SaveUnit(unit);
		}

		string inventoryData = JsonUtility.ToJson(boardState);
		string filePath = Application.persistentDataPath + boardStatePathName;
		Debug.Log(filePath);
		System.IO.File.WriteAllText(filePath, inventoryData);
		Debug.LogWarning("Saved board state.");
	}

	public void LoadBoardState()
	{
		string filePath = Application.persistentDataPath + boardStatePathName;
		string boardStateData = System.IO.File.ReadAllText(filePath);

		boardState = JsonUtility.FromJson<BoardState>(boardStateData);
		Debug.Log("Loaded board state.");
		Debug.Log("Loaded board state.");

		//GameContext.board.state = boardState;
	}
}

public static class SaveActions
{
	public static void SaveUnit(this BoardStateVariable boardStateVariable, Unit unit)
	{
		var newUnitState = new UnitState();
		newUnitState.name = unit.name;
		newUnitState.id = unit.gameObject.GetInstanceID();
		newUnitState.offsetPos = unit.OffsetPos;
		newUnitState.facing = unit.Facing;
		unit.state = newUnitState;
		boardStateVariable.unitStates.Add(newUnitState);
	}
}
