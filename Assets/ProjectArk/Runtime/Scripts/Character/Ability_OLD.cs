using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
	MOVEMENT,
	TARGET
}

public enum StepPhase
{
	UTILITY,
	MOVE,
	ATTACK
}

[RequireComponent(typeof(AbilityFlowController))]
public abstract class Ability_OLD : MonoBehaviour
{
	[Header("CONFIG:")]
	public Sprite icon;

	public string abilityName;

	public StepPhase phase;

	public AbilityType type;


	public virtual List<Cell_OLD> GetValidMoves(Cell_OLD cell, CharacterFlowController flow) => null;

	//public virtual Queue<CellObjectCommand> FetchCommands(Cell targetCell, CellObject cellObjct, FlowController flow)
	//{
	//	Queue<CellObjectCommand> newCommands = new Queue<CellObjectCommand>();
	//	return newCommands;
	//}

	public virtual Queue<CellObjectCommand> FetchCommandChain(
		Cell_OLD targetCell, 
		CellObject cellObj, 
		FlowController flow
		) 
	{
		Queue<CellObjectCommand> newCommands = new Queue<CellObjectCommand>();
		return newCommands;

		//Turn newTurn = new Turn();
		//newTurn.ability = this;
		//newTurn.instigator = cellObj;
		//return newTurn;

		//return  new Turn()
		//return Turn.CreateInstance(cellObj, this);
	}



	//... when you hover / unhover flow:
	Action peekValidMovesAction;
	public virtual void PeekValidMoves(Cell_OLD targetCell, CharacterFlowController flow)
	{
		Debug.LogWarning("peeking valid moves on " + this.gameObject.name);
	}

	public virtual void UnpeekValidMoves(Cell_OLD targetCell, CharacterFlowController flow)
	{
		Debug.LogWarning("unpeeking  valid moves on " + this.gameObject.name);
	}


	//... when you enter / exit flow:
	Action showValidMovesAction;
	public virtual void ShowValidMoves(Cell_OLD targetCell, CharacterFlowController flow) { }

	public virtual void HideValidMoves(Cell_OLD targetCell, CharacterFlowController flow) { }


	//... when you hover / unhover specific valid moves
	Action peekMovesAction;
	public virtual void PeekMove(Cell_OLD targetCell, CharacterFlowController flow) { }

	public virtual void UnpeekMove(Cell_OLD targetCell, CharacterFlowController flow) { }


	protected Action peekAction;
	public virtual void Peek(Cell_OLD targetCell, CharacterFlowController flow) { }

	public virtual void Unpeek() { }

	public AbilityFlowController _flow;
	public AbilityFlowController flow
	{
		get
		{
			if (_flow == null)
				_flow = GetComponent<AbilityFlowController>();
			return _flow;
		}
	}
}
