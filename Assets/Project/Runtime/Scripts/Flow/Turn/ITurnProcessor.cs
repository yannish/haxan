using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * some of these names seem odd.
 * RecordTurn is just taking in a turn and starting to process
 * ... doesn't put them in a record.
 */

public enum TurnPlaybackState
{
    PAUSED,
    PLAYING,
    REWINDING
}

public interface ITurnProcessor
{
    void ProcessTurns();

    void SetPhase(TeamPhase teamPhase);

    TeamPhase CurrPhase { get; }

    //...   soon as you record a turn, processor will return IsProcessing,
    //      until the turn is finished being executed.
    //void InputTurn(Turn turn);
    void InputCommands(CellObject instigator, Queue<CellObjectCommand> commands);

    void Undo();

    //void ProcessEnemyTurns();

    bool IsProcessing { get; }

    TurnPlaybackState CurrentState { get; }
}


//public void Undo()
//{
//	if (isProcessing)
//		return;

//	if (turnHistory.IsNullOrEmpty())
//	{
//		Debug.LogWarning("... no turns to undo!");
//		return;
//	}

//	Turn turnToUndo = turnHistory.Pop();
//	while (turnToUndo.commandHistory.Count > 0)
//	{
//		CellObjectCommand commandToUndo = turnToUndo.commandHistory.Pop();
//		commandToUndo.Undo();
//		turnToUndo.undoneCommands.Push(commandToUndo);
//	}

//	undoneTurns.Push(turnToUndo);

//	//if(turnToUndo.instigator is Character)
//	//{
//	//Character character = turnToUndo.instigator;
//	//character.flowController.
//	//}
//}