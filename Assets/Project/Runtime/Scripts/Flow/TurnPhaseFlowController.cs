using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPhaseFlowController : FlowController
{
	public float turnFlipTime;
	[ReadOnly] public float currTime;

	public override void Enter()
	{
		base.Enter();
		currTime = turnFlipTime;

		Globals.EventSystem.gameObject.SetActive(false);
	}

	public override void Exit()
	{
		base.Exit();

		Globals.EventSystem.gameObject.SetActive(true);
	}

	public override FlowState Tick()
	{
		base.Tick();

		currTime -= Time.deltaTime;
		if (currTime < 0f)
			return FlowState.DONE;

		return FlowState.RUNNING;
	}
}
