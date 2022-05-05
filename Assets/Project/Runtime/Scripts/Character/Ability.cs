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
		return Turn.CreateInstance(cellObj, this);
	}

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
