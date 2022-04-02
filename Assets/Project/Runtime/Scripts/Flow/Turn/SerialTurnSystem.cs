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

public class SerialTurnSystem : MonoBehaviour, ITurnProcessor
{
	[ReadOnly] public bool isProcessing;
	public bool IsProcessing => isProcessing;

	[ReadOnly] public TeamPhase phase;
	[ReadOnly, SerializeField] Enemy currEnemy;
	[ReadOnly, SerializeField] List<Enemy> enemyTurnOrder = new List<Enemy>();
	public void SetPhase(TeamPhase newPhase)
	{
		Globals.ReadyWanderers.Items?.Clear();
		Globals.ReadyEnemies.Items?.Clear();

		phase = newPhase;

		switch (phase)
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


	public void RecordTurn(Turn turn)
	{
		isProcessing = true;

		currTurn = turn;
		currCommand = turn.commands.Dequeue();
		currCommand.OnBeginTick();
	}

	public void ProcessTurns()
	{
		if(phase == TeamPhase.ENEMY)
		{
			if (currEnemy != null)
			{

			}
		}

		if (currCommand == null)
			return;

		if (currCommand.Tick())
		{
			currCommand.OnCompleteTick();
			currCommand.Execute();
			commandHistory.Push(currCommand);
			currCommand = null;
			if (!currTurn.commands.IsNullOrEmpty())
			{
				currCommand = currTurn.commands.Dequeue();
				currCommand.OnBeginTick();
			}
			else
			{
				isProcessing = false;
			}
		}
	}


	[Header("UNDO / REDO")]
	[ReadOnly] public Turn currTurn;
	[ReadOnly] public CharacterCommand currCommand;
	public Stack<CharacterCommand> commandHistory = new Stack<CharacterCommand>();
	public Stack<CharacterCommand> commandUndoHistory = new Stack<CharacterCommand>();

	public EditorButton undoBtn = new EditorButton("Undo", true);
	public void Undo()
	{
		if(commandHistory.IsNullOrEmpty())
		{
			Debug.LogWarning("no commands to undo!");
			return;
		}

		CharacterCommand currUndoCommand = commandHistory.Pop();
		currUndoCommand.Undo();
		commandUndoHistory.Push(currUndoCommand);
	}

	public EditorButton redoBtn = new EditorButton("Redo", true);
	public void Redo()
	{
		if (commandUndoHistory.IsNullOrEmpty())
		{
			Debug.LogWarning("no commands to redo!");
			return;
		}

		CharacterCommand currRedoCommand = commandUndoHistory.Pop();
		currRedoCommand.Execute();
		commandHistory.Push(currRedoCommand);
	}

	public void ProcessEnemyTurns()
	{
		
	}

	public List<Turn> playerTurns = new List<Turn>();
	public List<Turn> enemyTurns = new List<Turn>();

}
