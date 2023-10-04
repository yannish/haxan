using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameFlowState
{
	LOADING,
	RUNNING,
	PAUSED
}



public class GameContext : MonoBehaviour
{
	public static Action OnSaveBoardState;
	public static Action OnBoardStateSaveComplete;

	public static Action OnClearBoard;
	public static Action OnLoadBoardStte;

	public static Action<GameFlowState> OnGameStateChangeStart;
	public static Action<GameFlowState> OnGameStateChangeComplete;


	public GameFlowState state;

	public EditorButton saveBtn = new EditorButton("SaveBoardState");
	public EditorButton loadBtn = new EditorButton("LoadBoardState");


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
	{
		Debug.LogWarning("BOOT STRAPPED GAME CONTEXT");

		if (I != null)
			return;

		var contextInstanceObj = Instantiate(Resources.Load("Systems")) as GameObject;
		I = contextInstanceObj.GetComponent<GameContext>();

		DontDestroyOnLoad(contextInstanceObj);
	}


	public static GameContext I;
	private void Awake()
	{
		if(I != null)
		{
			Destroy(this.gameObject);
			return;
		}

		I = this;
		DontDestroyOnLoad(this.gameObject);
	}

	void SetState(GameFlowState newState)
	{
		OnGameStateChangeStart?.Invoke(newState);
		state = newState;
		OnGameStateChangeComplete?.Invoke(newState);
		Debug.LogWarning($"Switch game flow to {newState}");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
			SaveBoardState();

		if (Input.GetKeyDown(KeyCode.L))
			LoadBoardState();

		switch (state)
		{
			case GameFlowState.LOADING:
				break;
			case GameFlowState.RUNNING:
				TickRunningState();
				break;
			case GameFlowState.PAUSED:
				TickPausedState();
				break;
			default:
				break;
		}
	}

	void SaveBoardState()
	{
		OnSaveBoardState?.Invoke();
		//OnBoardStateSaveComplete?.Invoke();
	}

	void LoadBoardState()
	{
		OnClearBoard?.Invoke();
		OnLoadBoardStte?.Invoke();
	}


	private void TickPausedState()
	{

	}

	private void TickRunningState()
	{

	}
}
