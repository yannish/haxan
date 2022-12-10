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

	public List<Cell> peekedMoves = new List<Cell>();
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

		if (peekValidMovesAction != null)
		{
			peekValidMovesAction.Invoke();
			peekValidMovesAction = null;
		}


		validMoves = ability.GetValidMoves(characterFlow.character.currCell, characterFlow);
		if (validMoves.IsNullOrEmpty())
			return;

		validElements = validMoves.Select(t => t.GetComponent<UIElement>()).ToList();
		switch (ability.type)
		{
			case AbilityType.MOVEMENT:
				previewValidMovesControl = CellActions.EffectCells<CellPeekPathCommand>(validMoves);
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

		if (!validMoves.IsNullOrEmpty())
			validMoves.Clear();

		if(!validElements.IsNullOrEmpty())
			validElements.Clear();
	}


	Action peekValidMovesAction;

	public override void HoverPeek()
	{
		base.HoverPeek();

		Debug.LogWarning("hover peek in ability", this.gameObject);

		peekedMoves = ability.GetValidMoves(characterFlow.character.currCell, characterFlow);
		if (peekedMoves.IsNullOrEmpty())
			return;

		peekValidMovesAction = CellActions.EffectCells<CellPeekClickableCommand>(peekedMoves);

		ability.PeekValidMoves(characterFlow.character.currCell, characterFlow);
	}


	public override void HoverUnpeek()
	{
		base.HoverUnpeek();

		Debug.LogWarning("unhover peek in ability", this.gameObject);

		if (peekValidMovesAction != null)
		{
			peekValidMovesAction.Invoke();
			peekValidMovesAction = null;
		}

		ability.UnpeekValidMoves(characterFlow.character.currCell, characterFlow);
	}


	public override bool HandleHover(ElementHoveredEvent e)
	{
		//if (logDebug)
			Debog.logGameflow("... handling hover in abilityFlow " + this.ability.name);

		ability.Unpeek();

		if (
			//!validElements.IsNullOrEmpty() && 
			validElements.Contains(e.element)
			)
		{
			Debog.logGameflow("... it was valid!");

			var targetCell = e.element.GetComponent<Cell>();
			if (targetCell != null)
			{
				ability.Peek(targetCell, characterFlow);
				return true;
			}
		}

		//return base.HandleHover(e);

		//if (e.element.flowController == null)
		//	return false;

		//if (!(e.element.flowController is CellFlowController))
		//	return false;

		return false;
	}

	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		var clickedFlow = e.element.flowController;

		if (
			clickedFlow is CharacterFlowController
			&& clickedFlow == characterFlow
			)
		{
			Debug.LogWarning("CLICKED OWNER FROM WITHIN ABILITY.");
			return FlowState.DONE;
		}

		if (validElements.IsNullOrEmpty())
			return FlowState.YIELD;

		Cell cell = e.element.GetComponent<Cell>();
		if (cell == null)
			return FlowState.YIELD;

		if(validElements.Contains(e.element))
		{
			Turn newTurn = ability.FetchCommandChain(cell, characterFlow.character, characterFlow);
			
			characterFlow.ProvideInputTurn(newTurn);
			ability.Unpeek();

			return FlowState.RUNNING;
		}

		if (clickedFlow is AbilityFlowController)
			return FlowState.YIELD;

		if (clickedFlow is CharacterFlowController)
			return FlowState.DONE;

		return FlowState.YIELD;
	}
}
