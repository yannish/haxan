using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnButtonFlowController : FlowController
{
	public float turnFlipTime;
	[ReadOnly] public float currTime;

	//[ReadOnly, SerializeField] private TurnButton button;  

	//protected override void Awake()
	//{
		//button = GetComponent<TurnButton>();
		//if (button == null)
		//	return;

		//OnFlowPeeked += button.Hover;
		//OnFlowUnpeeked += button.Unhover;

		//OnFlowEntered += button.Click;
		//OnFlowExited += button.Unclick;
	//}

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
