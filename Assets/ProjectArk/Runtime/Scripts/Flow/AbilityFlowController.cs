using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityFlowController : FlowController
{
	//public Action previewValidMovesControl;

	Action peekValidMovesAction;

	Action previewValidMoveAction;

	public Ability_OLD _ability;

	public Ability_OLD ability
	{
		get
		{
			if (_ability == null)
				_ability = GetComponent<Ability_OLD>();
			return _ability;
		}
	}

	[ReadOnly] public AbilityScrObj abilityScrObj;

	public List<Cell_OLD> peekedMoves = new List<Cell_OLD>();

	public List<Cell_OLD> validMoves = new List<Cell_OLD>();
	
	public List<UIElement> validElements = new List<UIElement>();

	public CharacterFlowController characterFlow;
	
	public void ProvideCharacterFlow(CharacterFlowController characterFlow)
	{
		this.characterFlow = characterFlow;
	}


	protected override void Awake()
	{
		base.Awake();

		validMoves = new List<Cell_OLD>();
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

		validMoves = abilityScrObj.GetValidMoves(characterFlow.character.currCell, characterFlow);
		if (validMoves.IsNullOrEmpty())
			return;

		validElements = validMoves.Select(t => t.GetComponent<UIElement>()).ToList();

		switch (abilityScrObj.type)
		{
			case AbilityType_OLD.MOVEMENT:
				peekValidMovesAction = CellActions.EffectCells<CellPeekPathCommand>(validMoves);
				break;

			case AbilityType_OLD.TARGET:
				peekValidMovesAction = CellActions.EffectCells<CellClickableCommand>(validMoves);
				break;
		}
	}


	public override void Exit()
	{
		base.Exit();

		Debug.LogWarning("EXITING ABILITY FLOW : " + this.gameObject.name, this.gameObject);

		if(peekValidMovesAction != null)
		{
			peekValidMovesAction.Invoke();
			peekValidMovesAction = null;
		}

		if (previewValidMoveAction != null)
		{
			previewValidMoveAction.Invoke();
			previewValidMoveAction = null;
		}

		if (!validMoves.IsNullOrEmpty())
			validMoves.Clear();

		if(!validElements.IsNullOrEmpty())
			validElements.Clear();
	}


	public override void HoverPeek()
	{
		base.HoverPeek();

		Debug.LogWarning("hover peek in ability", this.gameObject);

		peekedMoves = abilityScrObj.GetValidMoves(characterFlow.character.currCell, characterFlow);
		if (peekedMoves.IsNullOrEmpty())
			return;

		peekValidMovesAction = CellActions.EffectCells<CellPeekClickableCommand>(peekedMoves);

		//ability.PeekValidMoves(characterFlow.character.currCell, characterFlow);
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

		//ability.UnpeekValidMoves(characterFlow.character.currCell, characterFlow);
	}


	public override bool HandleHoverStart(ElementHoveredEvent e)
	{
		//if (logDebug)
		//Debog.logInput("... handling hover in abilityFlow " + this.ability.name);

		if (peekValidMovesAction != null)
		{
			peekValidMovesAction.Invoke();
			peekValidMovesAction = null;
		}

		if (previewValidMoveAction != null)
		{
			previewValidMoveAction.Invoke();
			previewValidMoveAction = null;
		}

		//ability.Unpeek();

		if (validElements.Contains(e.element))
		{
			var targetCell = e.element.GetComponent<Cell_OLD>();
			if (targetCell != null)
			{
				previewValidMoveAction = abilityScrObj.Peek(targetCell, characterFlow);
				//ability.Peek(targetCell, characterFlow);
				return true;
			}
		}

		if(
			e.element != null
			&& e.element.flowController != null 
			&& e.element.flowController is AbilityFlowController
			)
		{
			AbilityFlowController peekedAbilityFlow = e.element.flowController as AbilityFlowController;
			if(peekedAbilityFlow.abilityScrObj.type == AbilityType_OLD.TARGET)
			{
				peekedMoves = peekedAbilityFlow.abilityScrObj.GetValidMoves(characterFlow.character.currCell, characterFlow);
				if (!peekedMoves.IsNullOrEmpty())
					peekValidMovesAction = CellActions.EffectCells<CellPeekClickableCommand>(peekedMoves);
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

		Cell_OLD cell = e.element.GetComponent<Cell_OLD>();
		if (cell == null)
			return FlowState.YIELD;

		if(validElements.Contains(e.element))
		{
			Queue<CellObjectCommand> newCommands = ability.FetchCommandChain(cell, characterFlow.character, characterFlow);
			characterFlow.ProvideInputCommands(newCommands);

			if (peekValidMovesAction != null)
			{
				peekValidMovesAction.Invoke();
				peekValidMovesAction = null;
			}

			//ability.Unpeek();

			//Turn newTurn = ability.FetchCommandChain(cell, characterFlow.character, characterFlow);
			//characterFlow.ProvideInputTurn(newTurn);

			return FlowState.RUNNING;
		}

		if (clickedFlow is AbilityFlowController)
			return FlowState.YIELD;

		if (clickedFlow is CharacterFlowController)
			return FlowState.DONE;

		return FlowState.YIELD;
	}
}
