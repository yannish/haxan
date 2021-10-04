using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterFlowController : CellObjFlowController
{
    [ReadOnly] public Character character;

	[ReadOnly] public QuickStateMachine fsm;

    public static event Action<Character> OnCharacterHovered = delegate { };
    public static event Action OnCharacterUnhovered = delegate { };

    public static event Action<Character> OnCharacterSelected = delegate { };
    public static event Action OnCharacterDeselected = delegate { };


    public abstract bool TryGetCommandStack(ref Stack<CharacterCommand> commandStack);

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
        OnCharacterHovered(character);

	}

	public override void HoverUnpeek()
    {
		fsm?.SetTrigger(FSM.unhover);

		base.HoverUnpeek();
        OnCharacterUnhovered();
    }

	public override void Enter()
	{
		fsm?.SetTrigger(FSM.select);

		base.Enter();

        OnCharacterSelected(character);

		if (character.movementAbility)
			TransitionTo(character.movementAbility.flow);
	}

	public override void Exit()
	{
		fsm?.SetTrigger(FSM.deselect);

		base.Exit();

        OnCharacterDeselected();
	}

	public override void HandleEmptyInput(EmptyClickEvent e)
	{
		base.HandleEmptyInput(e);
	}

	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		//if there's already a subFlow, pass input through that:
		if (subFlow != null)
		{
			Debug.Log("... subflow on character controller");

			var subFlowState = subFlow.HandleInput(e, this);

			switch (subFlowState)
			{
				case FlowState.YIELD:
					break;

				case FlowState.DONE:
					TransitionTo(null);
					return FlowState.RUNNING;

				case FlowState.RUNNING:
					subFlow.Update();
					return FlowState.RUNNING;
			}
		}

		return FlowState.RUNNING;
	}

	public virtual void BeginTurn() { }
    public virtual void EndTurn() { }

}
