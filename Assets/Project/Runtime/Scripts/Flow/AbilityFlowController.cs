using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityFlowController : FlowController
{
	public Action previewValidMovesControl;

	public Ability _ability;
	public Ability ability
	{
		get
		{
			if (_ability == null)
				_ability = GetComponent<Ability>();
			return _ability;
		}
	}

	public List<Cell> validMoves = new List<Cell>();
	public List<UIElement> validElements = new List<UIElement>();

	public CharacterFlowController characterFlow;
	public void ProvideCharacter(CharacterFlowController characterFlow)
	{
		this.characterFlow = characterFlow;
	}



	protected override void Awake()
	{
		base.Awake();
		validMoves = new List<Cell>();
		validElements = new List<UIElement>();
	}


	public override void Enter()
	{
		base.Enter();

		validMoves = ability.GetValidMoves(characterFlow.character.currCell, characterFlow);
		if (validMoves.IsNullOrEmpty())
			return;

		validElements = validMoves.Select(t => t.GetComponent<UIElement>()).ToList();

		switch (ability.type)
		{
			case AbilityType.MOVEMENT:
				previewValidMovesControl = CellActions.EffectCells<CellHintPathCommand>(validMoves);
				break;

			case AbilityType.TARGET:
				previewValidMovesControl = CellActions.EffectCells<CellClickableCommand>(validMoves);
				break;
		}
	}


	public override void Exit()
	{
		base.Exit();

		if (previewValidMovesControl != null)
		{
			previewValidMovesControl.Invoke();
			previewValidMovesControl = null;
		}

		ability.Unpeek();

		if(!validMoves.IsNullOrEmpty())
			validMoves.Clear();

		if(!validElements.IsNullOrEmpty())
			validElements.Clear();
	}

	public override bool HandleHover(ElementHoveredEvent e)
	{
		if (logDebug)
			Debog.logGameflow("... handling hover in abilityFlow " + this.ability.name);

		ability.Unpeek();

		if(
			//!validElements.IsNullOrEmpty() && 
			validElements.Contains(e.element)
			)
		{
			var targetCell = e.element.GetComponent<Cell>();
			if(targetCell != null)
			{
				ability.Peek(targetCell, characterFlow);
				return true;
			}
		}

		return base.HandleHover(e);

		//if (e.element.flowController == null)
		//	return false;

		//if (!(e.element.flowController is CellFlowController))
		//	return false;

		//return false;
	}

	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		//if (parentController == null && !(parentController is CharacterFlow))
		//	return FlowState.YIELD;

		//CharacterFlow characterFlow = parentController as CharacterFlow;

		if (validElements.IsNullOrEmpty())
			return FlowState.YIELD;

		Cell cell = e.element.GetComponent<Cell>();
		if (cell == null)
			return FlowState.YIELD;

		if(validElements.Contains(e.element))
		{
			Turn newTurn = ability.FetchCommandChain(cell, characterFlow.character, characterFlow);
			//Queue<CharacterCommand> newCommands = ability.FetchCommandChain(cell, characterFlow);
			characterFlow.ProvideInputTurn(newTurn);
			ability.Unpeek();
		}

		return FlowState.YIELD;
	}
}
