using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WandererDisplaySlot : RectUIElement
	, IFlowable
{
	[Header("COMPONENTS: ")]
	public Image iconSlot;
	public Image actionTab;
	public Image moveTab;
	
	//[Header("WANDERER")]
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

		wanderer.flow.OnCharacterFlowPeeked += OnFlowPeeked;
		wanderer.flow.OnCharacterFlowUnpeeked += OnFlowUnpeeked;

		wanderer.flow.OnCharacterFlowEntered += OnFlowEntered;
		wanderer.flow.OnCharacterFlowExited += OnFlowExited;

		wanderer.OnCharacterChanged += OnCharacterChanged;
	}

	private void OnCharacterChanged(Character character)
	{
		if(character.CurrMove < character.maxMoves)
		{
			moveTab.gameObject.SetActive(false);
		}

		if(character.currActions < character.maxActions)
		{
			actionTab.gameObject.SetActive(false);
		}

		if(character.CurrMove == character.maxMoves)
		{
			moveTab.gameObject.SetActive(true);
		}

		if (character.currActions == character.maxActions)
		{
			actionTab.gameObject.SetActive(true);
		}
	}

	private void OnFlowPeeked(FlowController obj) => Hover();

	private void OnFlowUnpeeked(FlowController obj) => Unhover();

	private void OnFlowEntered(FlowController obj) => Highlight();

	private void OnFlowExited(FlowController obj) => Unhighlight();
}
