using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WandererSlot : RectUIElement, IFlowable
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

		CellObjFlowController.OnFlowPeeked += OnFlowPeeked;
		CellObjFlowController.OnFlowUnpeeked += OnFlowUnpeeked;

		CellObjFlowController.OnFlowEntered += OnFlowEntered;
		CellObjFlowController.OnFlowExited += OnFlowExited;

	}

	private void OnFlowPeeked(CellObjFlowController obj)
	{
		if (obj != wanderer.flow)
			return;

		Hover();
	}

	private void OnFlowUnpeeked(CellObjFlowController obj)
	{
		if (obj != wanderer.flow)
			return;

		Unhover();
	}

	private void OnFlowEntered(CellObjFlowController obj)
	{
		if (obj != wanderer.flow)
			return;

		Highlight();
		//Click();
	}

	private void OnFlowExited(CellObjFlowController obj)
	{
		if (obj != wanderer.flow)
			return;

		Unhighlight();
		//Unclick();
	}
}
