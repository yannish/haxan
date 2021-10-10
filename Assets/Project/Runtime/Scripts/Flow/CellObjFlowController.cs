using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObjFlowController : FlowController
{
	[ReadOnly]
	public CellObject baseCellObject;

	public static event Action<CellObject> OnObjectHovered = delegate { };
	public static event Action<CellObject> OnObjetUnhovered = delegate { };

	public static event Action<CellObject> OnObjectSelected = delegate { };
	public static event Action<CellObject> OnObjectDeselected = delegate { };


	protected override void Awake() => baseCellObject = GetComponent<CellObject>();

	public override void Enter()
	{
		baseCellObject?.CurrentCell?.cellFlow.fsm.SetTrigger(FSM.select);
		OnObjectSelected(baseCellObject);
	}

	public override void Exit()
	{
		baseCellObject?.CurrentCell?.cellFlow.fsm.SetTrigger(FSM.deselect);
		OnObjectDeselected(baseCellObject);

		if (subFlow != null)
		{
			subFlow.Exit();
			subFlow = null;
		}
	}

	public override void HoverPeek()
	{
		Debog.logGameflow("Hover peeking : " + gameObject.name);
		baseCellObject?.CurrentCell?.cellFlow.fsm.SetTrigger(FSM.clickable);
		OnObjectHovered(baseCellObject);
	}

	public override void HoverUnpeek()
	{
		Debog.logGameflow("Hover unpeeking : " + gameObject.name);
		baseCellObject?.CurrentCell?.cellFlow.fsm.SetTrigger(FSM.unclickable);
		OnObjetUnhovered(baseCellObject);
	}
}
