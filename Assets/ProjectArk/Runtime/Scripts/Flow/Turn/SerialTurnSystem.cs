using BOG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum TeamPhase
{
	PLAYER,
	ENEMY,
	VICTORY
}

public class SerialTurnSystem : MonoBehaviour
	, ITurnProcessor
{

	//[ReadOnly] public bool isProcessing;
	public bool IsProcessing => currPlaybackState != TurnPlaybackState.PAUSED;

	[ReadOnly] public TurnPlaybackState currPlaybackState = TurnPlaybackState.PAUSED;
	public TurnPlaybackState CurrentState => currPlaybackState;

	[ReadOnly] public TeamPhase currPhase;
	public TeamPhase CurrPhase => currPhase;


	[Header("ENEMY PHASE:")]
	[ReadOnly, SerializeField] Enemy currEnemy;
	[ReadOnly, SerializeField] List<Enemy> enemyTurnOrder = new List<Enemy>();


	[Header("UNDO / REDO:")]

	//... FORWARD TURN PROCESSING:
	[ReadOnly] public CellObject currInstigator;

	[ReadOnly] public CellObjectCommand currCommand;
	
	public Queue<CellObjectCommand> commandsToProcess;

	public Stack<CellObjectCommand> currCommandHistory = new Stack<CellObjectCommand>();

	public Turn_OLDER currTurn;


	//... FORWARD BACKWARD PROCESSING:
	//[ReadOnly] public CellObjectCommand currBackwardCommand;

	//public Queue<CellObjectCommand> commandsToProcess;

	//public Stack<CellObjectCommand> currCommandHistory = new Stack<CellObjectCommand>();


	//... HISTORY:
	public Stack<Turn_OLDER> turnHistory = new Stack<Turn_OLDER>();

	public Stack<Turn_OLDER> undoneTurns = new Stack<Turn_OLDER>();
	
	public Stack<CellObjectCommand> commandUndoHistory = new Stack<CellObjectCommand>();

	//public Stack<CellObjectCommand> commandHistory = new Stack<CellObjectCommand>();


	public void SetPhase(TeamPhase newPhase)
	{
		Globals.ReadyWanderers.Items?.Clear();
		Globals.ReadyEnemies.Items?.Clear();

		currPhase = newPhase;

		switch (currPhase)
		{
			case TeamPhase.ENEMY:

				if(!Globals.ActiveEnemies.Items.IsNullOrEmpty())
				{
					foreach (var enemy in Globals.ActiveEnemies.Items)
					{
						enemy.Ready();
						Globals.ReadyEnemies.Add(enemy);
					}

					enemyTurnOrder = Globals.ReadyEnemies.Items
						.Cast<Enemy>()
						.OrderBy(t => t.turnPriority)
						.ToList();

					currEnemy = enemyTurnOrder.FirstOrDefault();

					break;
				}
				else
				{
					Debog.logGameflow("Tried switching to Enemy Phase, but no active enemies!");

					if (!Globals.ActiveWanderers.Items.IsNullOrEmpty())
					{
						SetPhase(TeamPhase.PLAYER);
					}

					break;
				}

			case TeamPhase.PLAYER:

				foreach (var wanderer in Globals.ActiveWanderers.Items)
				{
					wanderer.Ready();
					Globals.ReadyWanderers.Add(wanderer);
				}

				break;
		}
	}

	public void InputCommands(CellObject instigator, Queue<CellObjectCommand> commands)
	{
		Debug.LogWarning("INPUTTING COMMANDS");

		//isProcessing = true;
		currPlaybackState = TurnPlaybackState.PLAYING;

		currInstigator = instigator;
		commandsToProcess = commands;
		currCommand = commandsToProcess.Dequeue();
		currCommand.OnBeginTick();
		currCommandHistory = new Stack<CellObjectCommand>();
	}

	public void ProcessTurns()
	{
		//if(phase == TeamPhase.ENEMY)
		//{
		//	if (currEnemy != null)
		//	{
		//		Debug.LogWarning("Processing enemy: " + currEnemy.name);
		//	}
		//}

		switch (currPlaybackState)
		{
			case TurnPlaybackState.PAUSED:
				break;
			
			case TurnPlaybackState.PLAYING:
				ProcessTurnForward();
				break;
			
			case TurnPlaybackState.REWINDING:
				ProcessTurnBackwards();
				break;
			
			default:
				break;
		}

		if (currCommand == null)
			return;

		//if (currCommand.Tick())
		//{
		//	currCommand.OnCompleteTick();
		//	currCommand.Execute();
		//	currCommandHistory.Push(currCommand);
		//	currCommand = null;

		//	if (!commandsToProcess.IsNullOrEmpty())
		//	{
		//		currCommand = commandsToProcess.Dequeue();
		//		currCommand.OnBeginTick();
		//	}
		//	else
		//	{
		//		isProcessing = false;

		//		Turn recordedTurn = new Turn();
		//		recordedTurn.instigator = currInstigator;
		//		recordedTurn.commandHistory = currCommandHistory;
		//		turnHistory.Push(recordedTurn);

		//		Debug.LogWarning("... pushed turn to history: " + currCommandHistory.Count);

		//		currCommandHistory = null;
		//	}
		//}
	}

	void ProcessTurnForward()
	{
		if (currCommand == null)
			return;

		if (currCommand.Tick())
		{
			currCommand.OnCompleteTick();
			currCommand.Execute();
			currCommandHistory.Push(currCommand);
			currCommand = null;

			if (!commandsToProcess.IsNullOrEmpty())
			{
				currCommand = commandsToProcess.Dequeue();
				currCommand.OnBeginTick();
			}
			else
			{
				//isProcessing = false;
				currPlaybackState = TurnPlaybackState.PAUSED;

				Turn_OLDER recordedTurn = new();
				recordedTurn.instigator = currInstigator;
				recordedTurn.commandHistory = currCommandHistory;
				turnHistory.Push(recordedTurn);

				Debug.LogWarning("... pushed turn to history: " + currCommandHistory.Count);

				currCommandHistory = null;
			}
		}
	}

	void ProcessTurnBackwards()
	{
		if (currCommand == null)
			return;

		if (currCommand.Tick(-1f))
		{
			currCommand.OnCompleteReverseTick();
			//currCommand.OnCompleteTick();
			
			currCommand.Undo();
			//currCommand.Execute();

			//commandsToProcess.Enqueue(currCommand);
			//currCommandHistory.Push(currCommand);

			currCommand = null;

			if (!currCommandHistory.IsNullOrEmpty())
			{
				currCommand = currCommandHistory.Pop();
				currCommand.OnBeginReverseTick();
			}
			else
			{
				//isProcessing = false;
				currPlaybackState = TurnPlaybackState.PAUSED;

				currTurn.commands = commandsToProcess;
				currTurn.commandHistory = null;

				//Turn recordedTurn = new Turn();
				//recordedTurn.instigator = currInstigator;
				//recordedTurn.commandHistory = currCommandHistory;
				//turnHistory.Push(recordedTurn);

				Debug.LogWarning("... undid turn " + currCommandHistory.Count);

				currCommandHistory = null;
			}
		}
	}


	//public EditorButton undoBtn = new EditorButton("UNDO TURN", true);
	public void Undo()
	{
		if (IsProcessing)
			return;

		if(turnHistory.IsNullOrEmpty())
		{
			Debug.LogWarning("... no turns to undo!");
			return;
		}

		Turn_OLDER turnToUndo = turnHistory.Pop();
		currCommandHistory = turnToUndo.commandHistory;
		currCommand = currCommandHistory.Pop();

		currPlaybackState = TurnPlaybackState.REWINDING;

		//while (currCommandHistory.Count > 0)
		//{
		//	CellObjectCommand commandToUndo = turnToUndo.commandHistory.Pop();
		//	commandToUndo.Undo();
		//	turnToUndo.undoneCommands.Push(commandToUndo);
		//}

		//undoneTurns.Push(turnToUndo);

		//if(turnToUndo.instigator is Character)
		//{
			//Character character = turnToUndo.instigator;
			//character.flowController.
		//}
	}

	//public EditorButton redoBtn = new EditorButton("REDO TURN", true);
	public void Redo()
	{
		if (IsProcessing)
			return;

		if (undoneTurns.IsNullOrEmpty())
		{
			Debug.LogWarning("... no turns to redo!");
			return;
		}

		Turn_OLDER turnToRedo = undoneTurns.Pop();
		while(turnToRedo.undoneCommands.Count > 0)
		{
			CellObjectCommand commandToRedo = turnToRedo.undoneCommands.Pop();
			commandToRedo.Execute();
			turnToRedo.commandHistory.Push(commandToRedo);
		}

		turnHistory.Push(turnToRedo);
	}

#if UNITY_EDITOR

#endif
}
