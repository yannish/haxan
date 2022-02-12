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
		phase = newPhase;
		switch (phase)
		{
			case TeamPhase.ENEMY:
				Globals.ReadyEnemies.Items.Clear();
				foreach (var enemy in Globals.ActiveEnemies.Items)
				{
					Globals.ReadyEnemies.Add(enemy);
				}

				enemyTurnOrder = Globals.ReadyEnemies.Items
					.Cast<Enemy>()
					.OrderBy(t => t.turnPriority)
					.ToList();

				currEnemy = enemyTurnOrder.FirstOrDefault();

				break;

			case TeamPhase.PLAYER:
				Globals.ReadyWanderers.Items.Clear();
				foreach (var wanderer in Globals.ActiveWanderers.Items)
				{
					Globals.ReadyWanderers.Add(wanderer);
				}
				break;
		}

	}


	[ReadOnly] public Turn currTurn;
	[ReadOnly] public CharacterCommand currCommand;

	public Queue<CharacterCommand> commandHistory = new Queue<CharacterCommand>();



	public void RecordTurn(Turn turn)
	{
		isProcessing = true;

		currTurn = turn;
		currCommand = turn.commands.Dequeue();
		currCommand.Start();
	}

	public void ProcessTurns()
	{
		if(phase == TeamPhase.ENEMY)
		{
			//if(currEnemy != null)
		}

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
