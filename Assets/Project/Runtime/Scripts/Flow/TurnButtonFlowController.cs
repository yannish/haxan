using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnButtonFlowController : FlowController
{
	public FloatReference turnFlipTime;
	[ReadOnly] public float currTime;

	public static event Action<FlowController> OnTurnButtonPeeked = delegate { };

	public static event Action<FlowController> OnTurnButtonUnpeeked = delegate { };
										  
	public static event Action<FlowController> OnTurnButtonEntered = delegate { };
										  
	public static event Action<FlowController> OnTurnButtonExited = delegate { };

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

	public override void HoverPeek()
	{
		base.HoverPeek();
		OnTurnButtonPeeked?.Invoke(this);
	}

	public override void HoverUnpeek()
	{
		base.HoverUnpeek();
		OnTurnButtonUnpeeked(this);
	}

	public override void Enter()
	{
		base.Enter();

		Debug.LogWarning("entered turn button flow");

		currTime = turnFlipTime;

		Globals.EventSystem.gameObject.SetActive(false);
	}

	public override void Exit()
	{
		base.Exit();

		Debug.LogWarning("exited turn button flow");

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
