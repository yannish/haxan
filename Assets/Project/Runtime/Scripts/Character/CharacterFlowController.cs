using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterFlowController : CellObjFlowController
{
	public event Action<FlowController> OnCharacterFlowPeeked = delegate { };
												 
	public event Action<FlowController> OnCharacterFlowUnpeeked = delegate { };
												 
	public event Action<FlowController> OnCharacterFlowEntered = delegate { };
												 
	public event Action<FlowController> OnCharacterFlowExited = delegate { };


	[Header("CHARACTER")]
    [ReadOnly] public Character character;

	[ReadOnly] public QuickStateMachine fsm;

	public Turn inputTurn;

	public Queue<CellObjectCommand> inputCommands;
	
	public bool inputTurnSet { get; private set; }


	protected override void Awake()
	{
		base.Awake();
		character = cellObject as Character;
		fsm = GetComponentInChildren<QuickStateMachine>();
	}


	public void ProvideInputCommands(Queue<CellObjectCommand> commands)
	{
		Debug.LogWarning("Providing Input commands");
		inputCommands = commands;
	}


	public virtual bool TryGetInputCommands(out Queue<CellObjectCommand> newCommands)
	{
		newCommands = new Queue<CellObjectCommand>();
		if(!inputCommands.IsNullOrEmpty())
		{
			Debug.LogWarning("NEW COMMANDS!");
			newCommands = inputCommands;
			inputCommands = null;
			return true;
		}

		return false;
	}


    public override void HoverPeek()
    {
		fsm?.SetTrigger(FSM.hover);
		OnCharacterFlowPeeked?.Invoke(this);

		base.HoverPeek();
	}

	public override void HoverUnpeek()
    {
		fsm?.SetTrigger(FSM.unhover);
		OnCharacterFlowUnpeeked?.Invoke(this);
		
		base.HoverUnpeek();
    }

	public override void Enter()
	{
		fsm?.SetTrigger(FSM.select);
		OnCharacterFlowEntered?.Invoke(this);
		
		base.Enter();

		//if (character.movementAbilityFlow)
		//	TransitionTo(character.movementAbilityFlow);
	}

	public override void Exit()
	{
		//Debug.LogWarning("EXITING CHAR FLOW : " + this.gameObject.name, this.gameObject);
		fsm?.SetTrigger(FSM.deselect);
		OnCharacterFlowExited?.Invoke(this);
	
		base.Exit();
	}


	public override FlowState HandleBackInput(ElementBackClickedEvent e, FlowController parentController = null)
	{
		return FlowState.DONE;
	}


	public override bool HandleHoverStart(ElementHoveredEvent e)
	{
		//Debog.logInput("... handling ability hover in characterflow");

		if (subFlow != null)
		{
			var result = subFlow.HandleHoverStart(e);
			if (result)
				return true;
		}

		//if (peekedFlow != null)
		//{
		//	peekedFlow.HoverUnpeek();
		//	peekedFlow = null;
		//}

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
					return FlowState.DONE;

					//TransitionTo(null);
					//return FlowState.RUNNING;

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
