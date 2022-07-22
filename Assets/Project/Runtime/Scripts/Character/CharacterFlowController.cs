using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterFlowController : CellObjFlowController
{
	[Header("CHARACTER")]
    [ReadOnly] public Character character;
	[ReadOnly] public QuickStateMachine fsm;

	//public static event Action<CharacterFlowController> Entered = delegate { };
	//public static event Action<CharacterFlowController> Exited = delegate { };


	public Turn inputTurn;
	public void ProvideInputTurn(Turn newTurn)
	{
		//Debug.LogWarning("Providing Input Turn");
		inputTurn = newTurn;
	}


	public virtual bool TryGetInputTurn(ref Turn newInputTurn)
	{
		//Debug.LogWarning("trying to get input turn");

		if (inputTurn != null)
		{
			Debug.LogWarning("got input turn");

			newInputTurn = inputTurn;
			inputTurn = null;
			return true;
		}

		return false;
	}


	protected override void Awake()
    {
        base.Awake();
        character = baseCellObject as Character;
		fsm = GetComponentInChildren<QuickStateMachine>();
    }


    public override void HoverPeek()
    {
		fsm?.SetTrigger(FSM.hover);
		base.HoverPeek();
	}

	public override void HoverUnpeek()
    {
		fsm?.SetTrigger(FSM.unhover);
		base.HoverUnpeek();
    }

	public override void Enter()
	{
		fsm?.SetTrigger(FSM.select);
		base.Enter();

		//if (character.movementAbility)
		//	TransitionTo(character.movementAbility.flow);
	}


	public override void Exit()
	{
		fsm?.SetTrigger(FSM.deselect);
		base.Exit();
	}


	public override FlowState HandleBackInput(ElementBackClickedEvent e, FlowController parentController = null)
	{
		return FlowState.DONE;
	}


	public override bool HandleHover(ElementHoveredEvent e)
	{
		//Debug.LogWarning("hndling ability hover in characterflow");

		//if (subFlow == null)
		//	return false;

		if(peekedFlow != null)
		{
			peekedFlow.HoverUnpeek();
			peekedFlow = null;
		}

		if (e.element == null || e.element.flowController == null)
			return false;

		peekedFlow = e.element.flowController;
		peekedFlow.HoverPeek();

		return true;

		//if (!(subFlow is AbilityFlowController))
		//	return false;

		//var abilityFlow = subFlow as AbilityFlowController;


		//if (abilityFlow.ability.type == AbilityType.MOVEMENT)
		//{
		//	//Debug.LogWarning("... had a movement subflow");
		//	subFlow.HandleHover(e);
		//	return false;
		//}

		//return base.HandleHover(e);
	}


	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		var clickedFlow = e.element.flowController;

		if (subFlow != null)
		{
			var subFlowState = subFlow.HandleInput(e, this);

			switch (subFlowState)
			{
				case FlowState.YIELD:
					break;

				case FlowState.DONE:
					TransitionTo(null);
					return FlowState.RUNNING;

				case FlowState.RUNNING:
					return FlowState.RUNNING;
			}
		}


		//... ex. if you clicked the same ability icon again
		if (subFlow == clickedFlow)
		{
			Debog.logGameflow("clicked existing element in char flow");

			TransitionTo(null);
			peekedFlow = e.element.flowController;
			peekedFlow.HoverPeek();

			return FlowState.RUNNING;
		}

		
		if (clickedFlow is CharacterFlowController)
			return FlowState.YIELD;


		if (clickedFlow is AbilityFlowController)
		{
			var abilityFlow = (clickedFlow as AbilityFlowController);
			TransitionTo(abilityFlow);
			return FlowState.RUNNING;
		}

		//TransitionTo(clickedFlow);

		return FlowState.YIELD;
	}

	public virtual void BeginTurn() { }

    public virtual void EndTurn() { }

}
