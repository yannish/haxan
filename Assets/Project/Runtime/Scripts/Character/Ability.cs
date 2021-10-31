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


public abstract class Ability : MonoBehaviour
{
	public StepPhase phase;

	public AbilityType type;

	public virtual List<Cell> GetValidMoves(Cell cell, CharacterFlow flow) => null;

	public virtual Turn FetchCommandChain(Cell targetCell, CharacterFlow flow) 
	{
		return Turn.CreateInstance(flow.character, this);
	}
	//public virtual Queue<CharacterCommand> FetchCommandChain(Cell targetCell, CharacterFlow flow) => null;

	public virtual void Peek(Cell targetCell, CharacterFlow flow) { }

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
