using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObjFlowController : FlowController
{
	[ReadOnly]
	public CellObject cellObject;

	public static event Action<CellObjFlowController> OnHoverPeeked = delegate { };
	public static event Action<CellObjFlowController> OnHoverUnpeeked = delegate { };

	public static event Action<CellObjFlowController> OnEntered = delegate { };
	public static event Action<CellObjFlowController> OnExited = delegate { };

	//public static event Action<CellObjFlowController> OnObjectEnabled = delegate { };
	//public static event Action<CellObjFlowController> OnObjectDisabled = delegate { };

	protected override void Awake() => cellObject = GetComponent<CellObject>();


	public override void Enter()
	{
		base.Enter();

		//baseCellObject?.currCell?.cellFlow.visuals.SetTrigger(FSMtrigger.select, true);
		//baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.select);

		OnEntered(this);

		//OnFlowEntered
		//base.OnFlowEntered(this);
		//OnFlowEntered(this);
	}

	public override void Exit()
	{
		base.Exit();
		//baseCellObject?.currCell?.cellFlow.visuals.SetTrigger(FSMtrigger.select, false);
		//baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.deselect);
		
		if (subFlow != null)
		{
			subFlow.Exit();
			subFlow = null;
		}

		if (peekedFlow != null)
		{
			peekedFlow.HoverUnpeek();
			peekedFlow = null;
		}

		OnExited(this);
	}

	public override void HoverPeek()
	{
		base.HoverPeek();

		if(logDebug)
			Debog.logGameflow("Hover peeking : " + gameObject.name);

		//if (IsEnterable)
		//{
		//	baseCellObject?.currCell?.cellFlow.visuals.SetTrigger(FSMtrigger.clickable);
		//}
		//else
		//{
		//	baseCellObject?.currCell?.cellFlow.HoverPeek();
		//}
		//baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.hover);
		//baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.hover);

		//baseCellObject?.currCell?.cellFlow.HoverUnpeek();
		//baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.clickable);
		//baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.clickable);

		OnHoverPeeked(this);
	}

	public override void HoverUnpeek()
	{
		base.HoverUnpeek();

		if (logDebug)
			Debog.logGameflow("Hover unpeeking : " + gameObject.name);

		//baseCellObject?.currCell?.cellFlow.visuals.SetTrigger(FSMtrigger.clickable);
		//baseCellObject?.currCell?.cellFlow.HoverUnpeek();
		//baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.unhover);
		//baseCellObject?.currCell?.cellFlow.visuals.Unclickable();
		//baseCellObject?.currCell?.cellFlow.fsm.SetTrigger(FSM.unclickable);

		OnHoverUnpeeked(this);
	}
}
