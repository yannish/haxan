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
public abstract class Ability : MonoBehaviour
{

	[Header("CONFIG:")]
	public Sprite icon;

	public string abilityName;

	public StepPhase phase;

	public AbilityType type;


	public virtual List<Cell> GetValidMoves(Cell cell, CharacterFlowController flow) => null;


	public virtual Turn FetchCommandChain(Cell targetCell, CellObject cellObj, FlowController flow) 
	{
		Turn newTurn = new Turn(cellObj);
		newTurn.ability = this;
		return newTurn;
		//return  new Turn()
		//return Turn.CreateInstance(cellObj, this);
	}



	//... when you hover / unhover flow:
	Action peekValidMovesAction;
	public virtual void PeekValidMoves(Cell targetCell, CharacterFlowController flow)
	{
		Debug.LogWarning("peeking valid moves on " + this.gameObject.name);
	}

	public virtual void UnpeekValidMoves(Cell targetCell, CharacterFlowController flow)
	{
		Debug.LogWarning("unpeeking  valid moves on " + this.gameObject.name);
	}


	//... when you enter / exit flow:
	Action showValidMovesAction;
	public virtual void ShowValidMoves(Cell targetCell, CharacterFlowController flow) { }

	public virtual void HideValidMoves(Cell targetCell, CharacterFlowController flow) { }


	//... when you hover / unhover specific valid moves
	Action peekMovesAction;
	public virtual void PeekMove(Cell targetCell, CharacterFlowController flow) { }

	public virtual void UnpeekMove(Cell targetCell, CharacterFlowController flow) { }


	protected Action peekAction;

	public virtual void Peek(Cell targetCell, CharacterFlowController flow) { }

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
