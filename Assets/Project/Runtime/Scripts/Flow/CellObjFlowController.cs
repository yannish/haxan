using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObjFlowController : FlowController
{
	[ReadOnly]
	public CellObject baseCellObject;

	public static event Action<CellObjFlowController> OnFlowPeeked = delegate { };
	public static event Action<CellObjFlowController> OnFlowUnpeeked = delegate { };

	public static event Action<CellObjFlowController> OnFlowEntered = delegate { };
	public static event Action<CellObjFlowController> OnFlowExited = delegate { };

	public static event Action<CellObjFlowController> OnObjectEnabled = delegate { };
	public static event Action<CellObjFlowController> OnObjectDisabled = delegate { };

	protected override void Awake() => baseCellObject = GetComponent<CellObject>();


	public override void Enter()
	{
		baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.select);
		OnFlowEntered(this);
	}

	public override void Exit()
	{
		baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.deselect);
		OnFlowExited(this);

		if (subFlow != null)
		{
			subFlow.Exit();
			subFlow = null;
		}
	}

	public override void HoverPeek()
	{
		if(logDebug)
			Debog.logGameflow("Hover peeking : " + gameObject.name);

		baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.hover);
		baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.clickable);
		
		OnFlowPeeked(this);
	}

	public override void HoverUnpeek()
	{
		if (logDebug)
			Debog.logGameflow("Hover unpeeking : " + gameObject.name);

		baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.unhover);
		baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.unclickable);
		
		OnFlowUnpeeked(this);
	}
}
