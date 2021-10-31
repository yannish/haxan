using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TeamPhase
{
	PLAYER,
	ENEMY,
	RESOLVE
}

public class SerialTurnSystem : MonoBehaviour, ITurnProcessor
{
	[ReadOnly] public bool isProcessing;
	public bool IsProcessing => isProcessing;

	[ReadOnly] public TeamPhase phase;
	public void SetPhase(TeamPhase teamPhase)
	{
		Globals.ReadyWanderers.Items.Clear();
		foreach(var wanderer in Globals.ActiveWanderers.Items)
		{
			Globals.ReadyWanderers.Add(wanderer);
		}
	}


	[ReadOnly] public Turn currTurn;
	[ReadOnly] public CharacterCommand currCommand;

	public void RecordTurn(Turn turn)
	{
		isProcessing = true;

		//playerTurnQueue.Enqueue(turn);
		currTurn = turn;
		currCommand = turn.commands.Dequeue();
		currCommand.Start();
	}

	public void ProcessTurns()
	{
		if (currCommand == null)
			return;

		if (currCommand.Tick())
		{
			currCommand.End();
			currCommand = null;
			if (!currTurn.commands.IsNullOrEmpty())
			{
				currCommand = currTurn.commands.Dequeue();
				currCommand.Start();
			}
			else
			{
				isProcessing = false;
			}
		}
	}

	public Queue<CharacterCommand> commandHistory = new Queue<CharacterCommand>();

	public void Undo()
	{
		
	}

	public void ProcessEnemyTurns()
	{

	}

	//public Queue<Turn> playerTurnQueue = new Queue<Turn>();
	public List<Turn> playerTurns = new List<Turn>();
	public List<Turn> enemyTurns = new List<Turn>();

}
