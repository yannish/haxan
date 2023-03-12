using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WandererFlowController : CharacterFlowController
{
	[ReadOnly] public Wanderer wanderer;

	Action peekValidMovesAction;

	Action previewValidMoveAction;

	public List<Cell_OLD> validMoves = new List<Cell_OLD>();

	public List<UIElement> validElements = new List<UIElement>();


	protected override void Awake()
	{
		base.Awake();
		wanderer = GetComponent<Wanderer>();
	}

	private void Start()
	{
		if(wanderer.movementAbilityFlow != null)
			wanderer.movementAbilityFlow.ProvideCharacterFlow(this);
		
		wanderer.abilityFlows.ForEach(t => t.ProvideCharacterFlow(this));

		//wanderer.movementAbility.flow.ProvideCharacter(this);
		//foreach (var ability in wanderer.abilities)
		//{
		//	ability.flow.ProvideCharacter(wanderer.flow);
		//}
	}


	public override void Enter()
	{
		base.Enter();

		//if (character.movementAbilityScrObj != null)
		//{
		//	validMoves = character.movementAbilityScrObj.GetValidMoves(wanderer.currCell, this);
		//	if (validMoves.IsNullOrEmpty())
		//		return;

		//	validElements = validMoves.Select(t => t.GetComponent<UIElement>()).ToList();

		//	peekValidMovesAction = CellActions.EffectCells<CellPeekPathCommand>(validMoves);

		//	//TransitionTo(character.movementAbilityFlow);
		//}
	}

	public override void Exit()
	{
		base.Exit();

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

		if (!validMoves.IsNullOrEmpty())
			validMoves.Clear();

		if (!validElements.IsNullOrEmpty())
			validElements.Clear();
	}

	public override void HoverPeek()
	{
		base.HoverPeek();

		Debug.LogWarning("peeking wanderer flow.");

		if (character.movementAbilityScrObj != null)
		{
			validMoves = character.movementAbilityScrObj.GetValidMoves(wanderer.currCell, this);
			if (validMoves.IsNullOrEmpty())
				return;

			validElements = validMoves.Select(t => t.GetComponent<UIElement>()).ToList();

			peekValidMovesAction = CellActions.EffectCells<CellPeekPathCommand>(validMoves);

			//TransitionTo(character.movementAbilityFlow);
		}
	}

	public override void HoverUnpeek()
	{
		base.HoverUnpeek();

		Debug.LogWarning("UNPEEKING wanderer flow.");

		if (peekValidMovesAction != null)
		{
			peekValidMovesAction.Invoke();
			peekValidMovesAction = null;
		}
	}

	public override bool HandleHoverStart(ElementHoveredEvent e)
	{
		//if (peekValidMovesAction != null)
		//{
		//	peekValidMovesAction.Invoke();
		//	peekValidMovesAction = null;
		//}

		//if (previewValidMoveAction != null)
		//{
		//	previewValidMoveAction.Invoke();
		//	previewValidMoveAction = null;
		//}

		Debug.LogWarning("handling hover start in  wanderer flow.");

		//... handle a move:
		if (validElements.Contains(e.element))
		{
			var targetCell = e.element.GetComponent<Cell_OLD>();
			if (targetCell != null)
			{
				previewValidMoveAction = character.movementAbilityScrObj.Peek(targetCell, this);
				//ability.Peek(targetCell, characterFlow);
				Debug.LogWarning("... found a valid cell.");

				return true;
			}
		}


		if (subFlow != null)
		{
			var result = subFlow.HandleHoverStart(e);
			if (result)
				return true;
		}

		if (peekedFlow != null)
		{
			peekedFlow.HoverUnpeek();
			peekedFlow = null;
		}

		if (e.element == null || e.element.flowController == null)
			return false;

		peekedFlow = e.element.flowController;
		peekedFlow.HoverPeek();

		return false;
	}


	public override bool HandleHoverStop(ElementHoveredEvent e)
	{
		Debug.LogWarning("handling hover stop in  wanderer flow.");

		if (e.element == null || e.element.flowController == null)
			return false;

		WandererFlowController wandererFlow = e.element.flowController as WandererFlowController;
		if (wandererFlow == this)
			return true;

		//CellFlowController cellFlow = e.element.flowController as CellFlowController;

		if (peekedFlow != null)
		{
			peekedFlow.HoverUnpeek();
			peekedFlow = null;
		}

		if (previewValidMoveAction != null)
		{
			previewValidMoveAction.Invoke();
			previewValidMoveAction = null;
		}

		return true;
	}


	//public bool Tick()
	//{
	//	if (Input.GetKeyDown(KeyCode.J))
	//		Debug.Log("keystroke in wanderer flow");

	//	return !inputCommandStack.IsNullOrEmpty();
	//}


	public override void HandleEmptyInput(EmptyClickEvent e)
	{
		if (subFlow == character.movementAbilityFlow)
			return;

		base.HandleEmptyInput(e);
	}

	//public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	//{
	//	return FlowState.YIELD;
	//}
}
