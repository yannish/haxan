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
	//public static event Action<FlowController> On = delegate { };

	//public static event Action<FlowController> OnFlowUnpeeked = delegate { };

	[ReadOnly] public TeamPhase currPhase;
	public ITurnProcessor turnProcessor;

	[ReadOnly] public SerialTurnSystem serialTurnSystem;

	public void Start()
	{
		turnProcessor = GetComponentInChildren<ITurnProcessor>();
		turnProcessor.SetPhase(TeamPhase.PLAYER);
	}

	#region TURN PROCESSING:
	//public Character currCharacter;
	//... the turn waiting to be processed:
	//public List<Turn> currInputTurns;
	//public Turn currInputTurn;
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
				TransitionTo(null);

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

		if (!turnProcessor.IsProcessing)
		{
			if (subFlow != null && subFlow is WandererFlowController)
			{
				var currWandererFlow = subFlow as WandererFlowController;
				if (currWandererFlow.TryGetInputCommands(out Queue<CellObjectCommand> newCommands))
				{
					if (logDebug)
					{
						string log = string.Format(
							"new commands from {0}, length {1}",
							currWandererFlow.name,
							newCommands.Count
							);

						Debog.logGameflow(log);
					}

					TransitionTo(null, true);

					turnProcessor.InputCommands(currWandererFlow.character, newCommands);
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

	public override bool HandleHoverStart(ElementHoveredEvent e)
	{
		if ((turnProcessor as Component) && turnProcessor.IsProcessing)
			return false;

		if (subFlow != null && subFlow.HandleHoverStart(e))
		{
			Debug.LogWarning("... hover start done in main.");
			return true;
		}

		//if (peekedFlow != null)
		//{
		//	peekedFlow.HoverUnpeek();
		//	peekedFlow = null;
		//}

		//... 
		if (e.element == null || e.element.flowController == null)
			return false;

		peekedFlow = e.element.flowController;
		peekedFlow.HoverPeek();

		return false;
	}

	public override bool HandleHoverStop(ElementHoveredEvent e)
	{
		if (subFlow != null && subFlow.HandleHoverStop(e))
		{
			Debug.LogWarning("aight, done in main.");
			return true;
		}

		if (peekedFlow != null)
		{
			peekedFlow.HoverUnpeek();
			peekedFlow = null;
		}

		return false;
	}

	public override FlowState HandleBackInput(ElementBackClickedEvent e, FlowController parentController = null)
	{
		if (subFlow != null)
		{
			//... you've right clicked the subflow you're currently in, keep it hovered:
			if (e.element.flowController == subFlow)
			{
				Debug.LogWarning("... you've right-cllicked the subflow you're in:");
				TransitionTo(null, false);

				peekedFlow = e.element.flowController;
				peekedFlow.HoverPeek();

				return FlowState.YIELD;
			}

			//... you've right clicked somewhere else, but have a subflow, it should probably fold up:
			var result = subFlow.HandleBackInput(e);
			if (result == FlowState.DONE)
			{
				Debug.LogWarning("... you've right-cllicked somewhere else:");

				TransitionTo(null, false);
				return FlowState.YIELD;
			}
		}

		return FlowState.RUNNING;
	}

	public override void HandleEmptyInput(EmptyClickEvent e)
	{
		if ((turnProcessor as Component) && turnProcessor.IsProcessing)
			return;

		base.HandleEmptyInput(e);
	}

	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		if ((turnProcessor as Component) && turnProcessor.IsProcessing)
			return FlowState.RUNNING;

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

	#endregion
}
