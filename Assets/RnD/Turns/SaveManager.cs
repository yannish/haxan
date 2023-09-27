using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
	public static Action<BoardState> OnBoardStateSaveStart;

	public static Action<BoardState> OnBoardStateSaveComplete;

	public static Action<BoardState> OnBoardStateLoadStart;

	public static Action<BoardState> OnBoardStateLoadComplete;

	const string boardStatePathName = "/BoardState.json";

	public BoardState boardState = new BoardState();

    //public MatchData matchData = new MatchData();

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			SaveBoardState();
		}

		if (Input.GetKeyDown(KeyCode.L))
		{
			LoadBoardState();
		}
	}

	public EditorButton saveBtn = new EditorButton("SaveBoardState");
    public void SaveBoardState()
	{
		boardState.unitStates.Clear();

		OnBoardStateSaveStart?.Invoke(boardState);

		//foreach(var unit in GameContext.activeUnits.Items)
		//{

		//}

		string inventoryData = JsonUtility.ToJson(boardState);
		string filePath = Application.persistentDataPath + boardStatePathName;
		Debug.Log(filePath);
		System.IO.File.WriteAllText(filePath, inventoryData);
		Debug.LogWarning("Saved board state.");

		OnBoardStateSaveComplete?.Invoke(boardState);
	}

	public EditorButton loadBtn = new EditorButton("LoadBoardState");
	public void LoadBoardState()
	{
		OnBoardStateLoadStart?.Invoke(boardState);

		string filePath = Application.persistentDataPath + boardStatePathName;
		string boardStateData = System.IO.File.ReadAllText(filePath);

		boardState = JsonUtility.FromJson<BoardState>(boardStateData);
		Debug.Log("Loaded board state.");

		OnBoardStateLoadComplete?.Invoke(boardState);
	}
}
