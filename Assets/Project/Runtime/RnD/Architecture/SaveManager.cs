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
		BoardState newBoardState = new BoardState();
		foreach (var unit in GameVariables.activeUnits.Items)
		{
			var cachedUnitState = unit.CacheState();
			newBoardState.unitStates.Add(cachedUnitState);
		}

		GameVariables.board.state = newBoardState;

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

		//GameVariables.board = boardState;
	}
}

public static class SaveActions
{
	public static UnitState CacheState(this Unit unit)
	{
		var newUnitState = new UnitState();

		newUnitState.templatePath = unit.templatePath;
		newUnitState.type = unit.type;
		newUnitState.offsetPos = unit.OffsetPos;
		newUnitState.facing = unit.Facing;
		//unit.state = newUnitState;

		return newUnitState;
		//boardStateVariable.unitStates.Add(newUnitState);
	}
}
