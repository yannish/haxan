using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BoardHistory
{
	public const int MAX_OPS = 100;
	public const int MAX_INSTIGATING_OPS = 100;
	public const int MAX_TURNS = 100;
	public const int MAX_TURN_STEPS = 100;

	[ReadOnly] public int turnCount = 0;
	[ReadOnly] public int totalCreatedTurnSteps = 0;
	[ReadOnly] public int totalCreatedOps = 0;

	public Turn[] turns = new Turn[MAX_TURNS];
	public TurnStep[] turnSteps = new TurnStep[MAX_TURN_STEPS];

	[SerializeReference]
	public UnitOp[] allOps = new UnitOp[MAX_OPS];

	public IUnitOperable[] allOps_OLD = new IUnitOperable[MAX_OPS];
	public UnitOp_STRUCT[] allOps_NEW = new UnitOp_STRUCT[MAX_OPS];

	//... TODO: 
	//... need to track bit of playback as well. when we load, we load at the turn we've played up to.
	public int currPlaybackTurn;

	public Turn currTurn => turns[currPlaybackTurn];
	public Turn prevTurn => turns[currPlaybackTurn - 1];
}

[Serializable]
public class CompressedBoardHistory
{
	public CompressedBoardHistory()
	{

	}

	public CompressedBoardHistory(BoardHistory history)
	{
		for (int i = 0; i < history.turnCount; i++)
		{
			turns.Add(history.turns[i]);
		}

		for (int i = 0; i < history.totalCreatedTurnSteps; i++)
		{
			turnSteps.Add(history.turnSteps[i]);
		}

		for (int i = 0; i < history.totalCreatedOps; i++)
		{
			ops.Add(history.allOps[i]);
		}
	}

	public List<Turn> turns = new List<Turn>();
	public List<TurnStep> turnSteps = new List<TurnStep>();

	[SerializeReference]
	public List<UnitOp> ops = new List<UnitOp>();
}