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

	public CharacterFlow characterFlow;
	public void ProvideCharacter(CharacterFlow characterFlow)
	{
		this.characterFlow = characterFlow;
	}

	public override void Enter()
	{
		base.Enter();

		validMoves = ability.GetValidMoves(characterFlow.character.CurrentCell, characterFlow);
		if (validMoves.IsNullOrEmpty())
			return;
		validElements = validMoves.Select(t => t.GetComponent<UIElement>()).ToList();

		switch (ability.type)
		{
			case AbilityType.MOVEMENT:
				previewValidMovesControl = CellActions.NewEffectCells<CellHintPathCommand>(validMoves);
				break;

			case AbilityType.TARGET:
				previewValidMovesControl = CellActions.NewEffectCells<CellClickableCommand>(validMoves);
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

		validMoves = null;
		validElements = null;
	}

	public override bool HandleHover(ElementHoveredEvent e)
	{
		ability.Unpeek();

		if(validElements.Contains(e.element))
		{
			var targetCell = e.element.GetComponent<Cell>();
			if(targetCell != null)
			{
				ability.Peek(targetCell, characterFlow);
				return true;
			}
		}

		return false;

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

		Cell cell = e.element.GetComponent<Cell>();
		if (cell == null)
			return FlowState.YIELD;


		if(validElements.Contains(e.element))
		{
			Queue<CharacterCommand> newCommands = ability.FetchCommandChain(cell, characterFlow);
			characterFlow.ProvideCommandStack(newCommands);
		}

		ability.Unpeek();

		return FlowState.DONE;
	}
}
