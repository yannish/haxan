using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WandererDisplaySlot : RectUIElement, IFlowable
{
	[Header("WANDERER")]
	public Image iconSlot;
	[ReadOnly] public Wanderer wanderer;

	public FlowController Flow => wanderer != null ? wanderer.flow : null;

	//[ReadOnly] public 
	//[ReadOnly] public QuickStateMachine fsm;

	//public override void Awake()
	//{
	//base.Awake();
	//fsm = GetComponentInChildren<QuickStateMachine>();
	//}

	public void BindTo(Wanderer wanderer)
	{
		this.wanderer = wanderer;
		this.iconSlot.sprite = wanderer.icon;

		//wanderer.flow.OnFlowPeeked += OnFlowPeeked;
		//wanderer.flow.OnFlowUnpeeked += OnFlowUnpeeked;

		//wanderer.flow.OnFlowEntered += OnFlowEntered;
		//wanderer.flow.OnFlowExited += OnFlowExited;

	}

	private void OnFlowPeeked(FlowController obj) => Hover();

	private void OnFlowUnpeeked(FlowController obj) => Unhover();

	private void OnFlowEntered(FlowController obj) => Highlight();

	private void OnFlowExited(FlowController obj) => Unhighlight();
}
