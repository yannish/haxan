using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterFlow : CellObjFlowController
{
    [ReadOnly] public Character character;
	[ReadOnly] public QuickStateMachine fsm;


	Turn inputTurn;
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

		if (character.movementAbility)
			TransitionTo(character.movementAbility.flow);
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

		if (subFlow == null)
			return false;

		if (!(subFlow is AbilityFlowController))
			return false;

		var abilityFlow = subFlow as AbilityFlowController;
		if (abilityFlow.ability.type == AbilityType.MOVEMENT)
		{
			//Debug.LogWarning("... had a movement subflow");
			subFlow.HandleHover(e);
			return false;
		}

		return base.HandleHover(e);
	}


	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		//if there's already a subFlow, pass input through that:
		if (subFlow != null)
		{
			var subFlowState = subFlow.HandleInput(e, this);

			switch (subFlowState)
			{
				case FlowState.YIELD:
					return FlowState.YIELD;

				case FlowState.DONE:
					TransitionTo(null);
					return FlowState.RUNNING;

				case FlowState.RUNNING:
					subFlow.Update();
					return FlowState.RUNNING;
			}
		}

		return FlowState.YIELD;
	}

	public virtual void BeginTurn() { }

    public virtual void EndTurn() { }

}
