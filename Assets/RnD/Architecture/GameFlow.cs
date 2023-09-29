using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameFlow : MonoBehaviour
{
	//public GameFlowState state;

	//public static Action OnBoardStateSaveStart;
	//public static Action OnBoardStateSaveComplete;

	//public static Action OnBoardStateLoadStart;
	//public static Action OnBoardStateLoadComplete;

	//public static Action<GameFlowState> OnGameStateChangeStart;
	//public static Action<GameFlowState> OnGameStateChangeComplete;


	//void SetState(GameFlowState newState)
	//{
	//	OnGameStateChangeStart?.Invoke(newState);
	//	state = newState;
	//	OnGameStateChangeComplete?.Invoke(newState);
	//	Debug.LogWarning($"Switch game flow to {newState}");
	//}

	//private void Update()
	//{
	//	if (Input.GetKeyDown(KeyCode.S))
	//	{
	//		OnBoardStateSaveStart?.Invoke();
	//		OnBoardStateSaveComplete?.Invoke();
	//	}

	//	if (Input.GetKeyDown(KeyCode.L))
	//	{
	//		OnBoardStateLoadStart?.Invoke();
	//		OnBoardStateLoadComplete?.Invoke();
	//	}

	//	switch (state)
	//	{
	//		case GameFlowState.LOADING:
	//			break;
	//		case GameFlowState.RUNNING:
	//			TickRunningState();
	//			break;
	//		case GameFlowState.PAUSED:
	//			TickPausedState();
	//			break;
	//		default:
	//			break;
	//	}
	//}

	//private void TickPausedState()
	//{
		
	//}

	//private void TickRunningState()
	//{
		
	//}
}
