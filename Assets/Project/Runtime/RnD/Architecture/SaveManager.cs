using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
	const string boardLayoutPathName = "/BoardLayout.json";
	const string boardHistoryPathName = "/BoardHistory.json";

	public BoardLayout boardState = new BoardLayout();
	public BoardHistory history = new BoardHistory();
	public CompressedBoardHistory compressedHistory = new CompressedBoardHistory();

	private void OnEnable()
	{
		GameContext.OnSaveBoardState += SaveBoardLayout;
		GameContext.OnSaveBoardState += SaveBoardHistory;

		GameContext.OnLoadBoardStte += LoadBoardLayout;
	}

	private void OnDisable()
	{
		GameContext.OnSaveBoardState -= SaveBoardLayout;
		GameContext.OnSaveBoardState -= SaveBoardHistory;

		GameContext.OnLoadBoardStte -= LoadBoardLayout;
	}

	private void SaveBoardLayout()
	{
		BoardLayout newBoardState = new BoardLayout();
		foreach (var unit in Haxan.activeUnits.Items)
		{
			var cachedUnitState = unit.CacheState();
			newBoardState.unitStates.Add(cachedUnitState);
		}

		Haxan.state.layout = newBoardState;
		string boardStateData = JsonUtility.ToJson(boardState);
		string filePath = Application.persistentDataPath + boardLayoutPathName;

		System.IO.File.WriteAllText(filePath, boardStateData);

		Debug.LogWarning($"Saved board state: {filePath}");
	}

	public void LoadBoardLayout()
	{
		string filePath = Application.persistentDataPath + boardLayoutPathName;
		string boardStateData = System.IO.File.ReadAllText(filePath);

		boardState = JsonUtility.FromJson<BoardLayout>(boardStateData);
		Debug.Log("Loaded board state.");

		//GameVariables.board = boardState;
	}

	private void SaveBoardHistory()
	{
		CompressedBoardHistory history = new CompressedBoardHistory(Haxan.history);
		string historyData = JsonUtility.ToJson(history);
		string filePath = Application.persistentDataPath + boardHistoryPathName;
		System.IO.File.WriteAllText(filePath, historyData);
		Debug.LogWarning($"Saved board history: {filePath}");

	}

	private void LoadBoardHistory()
	{
		string filePath = Application.persistentDataPath + boardHistoryPathName;
		string boardHistoryData = System.IO.File.ReadAllText(filePath);
		compressedHistory = JsonUtility.FromJson<CompressedBoardHistory>(boardHistoryData);
		Debug.LogWarning($"Loaded board history: {filePath}");
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
