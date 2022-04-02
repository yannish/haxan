using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WandererSlot : UIElement
{
	[Header("WANDERER")]
	public Image iconSlot;
	[ReadOnly] public Wanderer wanderer;
	[ReadOnly] public QuickStateMachine fsm;

	private void Awake()
	{
		fsm = GetComponentInChildren<QuickStateMachine>();
	}

	public void ProvideWanderer(Wanderer wanderer)
	{
		this.wanderer = wanderer;
		this.iconSlot.sprite = wanderer.icon;
	}

	public override FlowController flowController
	{
		get
		{
			if (wanderer != null)
				return wanderer.flowController;

			return null;
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		fsm?.SetTrigger(FSM.hover);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		fsm?.SetTrigger(FSM.unhover);
	}
}
