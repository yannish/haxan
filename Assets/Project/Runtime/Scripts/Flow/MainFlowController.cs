using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *... why put turnflow control right in here w/ mainflow...
 * 
 * when you've got a command chain in from whichever character (wanderer or enemy)
 * that's it's own flow, so makes sense to inject here?
 */

public class MainFlowController : FlowController
{
	public static void InsertNewTurn(Turn newTurn)
	{

	}

	[ReadOnly] public TeamPhase currPhase;

	public void Start()
	{
		turnProcessor = GetComponentInChildren<ITurnProcessor>();
		turnProcessor.SetPhase(TeamPhase.PLAYER);

		//dummyTurn = ScriptableObject.CreateInstance<DummyTurn>();
		//SetPhase(TeamPhase.PLAYER);
	}


	#region TURN PROCESSING:

	public ITurnProcessor turnProcessor;
	
    public Character currCharacter;

	//... the turn waiting to be processed:
	public Turn currInputTurn;

	#endregion


	public bool doBreak;

	private bool wasProcessingLastFrame;
	private void LateUpdate()
	{
		if (doBreak)
			doBreak = false;

		if ((turnProcessor as Component) == null)
			return;

		turnProcessor.ProcessTurns();
		if (turnProcessor.IsProcessing)
		{
			if (!wasProcessingLastFrame && subFlow != null)
			{
				TransitionTo(null);
			}

			wasProcessingLastFrame = true;
			return;
		}

		if (wasProcessingLastFrame && lastSubFlow != null)
		{
			wasProcessingLastFrame = false;
			TransitionTo(lastSubFlow, true);
		}

		if(subFlow != null)
		{
			var subflowTickResult = subFlow.Tick();
			if (subflowTickResult == FlowState.DONE)
			{
				TransitionTo(null);
				return;
			}
		}

		if (currInputTurn == null)
		{
			if (subFlow != null && subFlow is WandererFlowController)
			{
				var currWandererFlow = subFlow as WandererFlowController;
				if (currWandererFlow.TryGetInputTurn(ref currInputTurn))
				{
					if (logDebug)
					{
						string log = string.Format(
							"new turn from {0}, length {1}",
							currWandererFlow.name,
							currInputTurn.commands.Count
							);

						Debog.logGameflow(log);
					}

					TransitionTo(null, true);

					turnProcessor.RecordTurn(currInputTurn);

					currInputTurn = null;
				}
			}
		}

		//switch (currPhase)
		//{
		//	case TeamPhase.PLAYER:
		//		break;

		//	case TeamPhase.ENEMY:
		//		Debog.logGameflow("Enemy turn, back to player.");
		//		break;

		//	case TeamPhase.VICTORY:
		//		break;

		//	default:
		//		break;
		//}
	}


	#region FLOW PROCESSING:

	public override bool HandleHover(ElementHoveredEvent e)
	{
		if ((turnProcessor as Component) && turnProcessor.IsProcessing)
			return false;

		if (subFlow != null && subFlow.HandleHover(e))
			return true;

		if (peekedFlow != null)
		{
			peekedFlow.HoverUnpeek();
			peekedFlow = null;
		}

		//... 
		if (e.element == null || e.element.flowController == null)
			return false;

		peekedFlow = e.element.flowController;
		peekedFlow.HoverPeek();

		return false;
	}


	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		var clickedFlow = e.element.flowController;
		if (clickedFlow == null)
			return FlowState.RUNNING;


		//... ex. if you clicked the same character or ability
		if (subFlow == clickedFlow)
		{
			Debog.logGameflow("clicked existing element in mainflow");

			TransitionTo(null);

			peekedFlow = e.element.flowController;
			peekedFlow.HoverPeek();

			return FlowState.RUNNING;
		}


		if (subFlow != null)
		{
			var subflowState = subFlow.HandleInput(e, parentController);

			switch (subflowState)
			{
				case FlowState.RUNNING:
					return FlowState.RUNNING;

				case FlowState.DONE:
					TransitionTo(null);
					peekedFlow = e.element.flowController;
					peekedFlow.HoverPeek();
					//return FlowState.RUNNING;
					break;

				case FlowState.YIELD:
					break;
			}
		}


		if(clickedFlow.IsEnterable)
		{
			TransitionTo(clickedFlow);
		}
		else 
		{
			if (subFlow != null)
				TransitionTo(null);

			peekedFlow = e.element.flowController;
			peekedFlow.HoverPeek();

			return FlowState.RUNNING;
		}

		return FlowState.RUNNING;
	}

	//public override FlowState HandleInput(ElementClickedEvent e, FlowController owner)
	//{
	//	// TODO: how to nullcheck reference to interface..
	//	if ((turnProcessor as Component) && turnProcessor.IsProcessing)
	//		return FlowState.RUNNING;

	//	if(logDebug)
	//		Debog.logInput("Handling input on maincontroller");

	//	//if (currInputTurn != null)
	//	//{
	//	//	if (logDebug)
	//	//		Debog.logGameflow("... curr command stack still being processed");
	//	//	return FlowState.RUNNING;
	//	//}

 //       //if there's already a subFlow, pass input through that:
 //       if (subFlow != null)
	//	{
	//		var subFlowState = subFlow.HandleInput(e, this);

	//		switch (subFlowState)
	//		{
	//			case FlowState.YIELD:
	//				break;

	//			case FlowState.DONE:
	//				TransitionTo(null);
	//				return FlowState.RUNNING;

	//			case FlowState.RUNNING:
	//				//subFlow.Tick();
	//				return FlowState.RUNNING;
	//		}
	//	}

	//	if (logDebug)
	//		Debog.logGameflow("subflow passed on input in main");

	//	//... if subflow didn't want input, or you've clicked a new one:
	//	if (e.element.flowController != null)
	//	{
	//		Debog.logGameflow("clicked element had a subflow : " + e.element.flowController.name);

	//		if (e.element.flowController is TurnButtonFlowController)
	//		{
	//			if (logDebug)
	//				Debog.logInput("TURN BUTTON PRESSED");

	//			TransitionTo(null);

	//			turnProcessor.SetPhase(TeamPhase.ENEMY);

	//			return FlowState.RUNNING;
	//		}

	//		if(e.element.flowController is CellFlowController)
	//		{
	//			Debog.logGameflow("... it's cell flow: " + e.element.flowController);
	//			return FlowState.RUNNING;
	//		}


	//		//... HANDLE BELOW IN BASE:

	//		////... clicking the element you're already in:
 //  //         if (e.element.flowController == subFlow)
	//		//{
	//		//	Debog.logGameflow("clicked existing element");
	//		//	TransitionTo(null);

	//		//	//... go back to peeking it:
	//		//	peekedFlow = e.element.flowController;
	//		//	peekedFlow.HoverPeek();

	//		//	//... clear out "selected wanderer so that TurnFlow stops ticking it:
	//		//	//Globals.SelectedWanderer.Items.Clear();
	//		//}
	//		//else
	//		//{
	//		//	if(logDebug)
	//		//		Debog.logGameflow("found flow, heading in");

	//		//	TransitionTo(e.element.flowController);

	//		//	//... fill out "selected wanderer" for TurnFlow to reference:
	//		//	//if (e.element.flowController is WandererFlowController)
	//		//	//{
	//		//	//	Debug.Log("... setting selected wanderer");

	//		//	//	Wanderer wanderer = (e.element.flowController as WandererFlowController).wanderer;
	//		//	//	Globals.SelectedWanderer.Add(wanderer);
	//		//	//}
	//		//}
	//	}
	//	else
	//	{
	//		Debog.logGameflow("... element flow controller was null on maincontroller");
	//	}

	//	return base.HandleInput(e, this);

	//	//return FlowState.RUNNING;
	//}

	#endregion
}
