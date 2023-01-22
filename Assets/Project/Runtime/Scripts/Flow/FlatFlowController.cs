using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlatFlowState
{
    NEUTRAL,
    PEEKING_WANDERER,
    PEEKING_ABILITY,
}

public class FlatFlowController : MonoBehaviour
{
    [ReadOnly] public FlatFlowState flatFlowState = FlatFlowState.NEUTRAL;

    public WandererFlowController selectedWandererFlow;
    
    public WandererFlowController wandererPeekedThisFrame;
    public WandererFlowController wandererUnpeekedThisFrame;
	public WandererFlowController peekedWandererFlow;

	public EnemyFlowController peekedEnemyFlow;
    
    public AbilityFlowController abilityFlow;


    public void HandleHoverStart(ElementHoveredEvent e)
	{
        if (e.element == null || e.element.flowController == null)
            return;

        if(e.element.flowController is WandererFlowController)
            wandererPeekedThisFrame = e.element.flowController as WandererFlowController;
	}

	public void HandleHoverStop(ElementHoveredEvent e)
	{
        if (e.element == null || e.element.flowController == null)
            return;

		if (e.element.flowController is WandererFlowController)
			wandererUnpeekedThisFrame = e.element.flowController as WandererFlowController;
	}

	private void LateUpdate()
	{
		switch (flatFlowState)
		{
			case FlatFlowState.NEUTRAL:
				TickNeutralFlow();
				break;
			case FlatFlowState.PEEKING_WANDERER:
				TickPeekingWandererFlow();
				break;
			case FlatFlowState.PEEKING_ABILITY:
				TickPeekingAbility();
				break;
			default:
				break;
		}

		ClearFrame();
	}

	private void ClearFrame()
	{
		wandererPeekedThisFrame = null;
	}


	private void TickNeutralFlow()
	{
		if (peekedWandererFlow == null && wandererPeekedThisFrame != null)
		{
			peekedWandererFlow = wandererPeekedThisFrame;
			flatFlowState = FlatFlowState.PEEKING_WANDERER;
			//... do wanderer-peeking UI stuff...
			//PeekWanderer(wandererPeekedThisFrame);
			return;
		}
	}

	private void TickPeekingWandererFlow()
	{
		if(wandererUnpeekedThisFrame != null && wandererUnpeekedThisFrame == peekedWandererFlow)
		{

		}
	}


	private void TickPeekingAbility()
	{
		
	}

}
