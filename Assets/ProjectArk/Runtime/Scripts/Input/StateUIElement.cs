using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateUIElement : UIElement
{
	[ReadOnly] public QuickStateMachine fsm;
	public override  void Awake()
	{
		base.Awake();

		fsm = GetComponentInChildren<QuickStateMachine>();
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

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);

		if (fsm != null)
		{
			Debug.LogWarning("SETTING SELECT STATE");
			fsm.SetTrigger(FSM.select);
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		//fsm?.SetTrigger(FSM.deselect);
	}
}
