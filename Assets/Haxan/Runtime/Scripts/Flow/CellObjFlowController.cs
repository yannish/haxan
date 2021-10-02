using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObjFlowController : FlowController
{
	[ReadOnly]
	public CellObject baseCellObject;

	protected override void Awake() => baseCellObject = GetComponent<CellObject>();

	public override void Enter()
	{
		baseCellObject?.CurrentCell?.cellFlow.fsm.SetTrigger(FSM.select);
	}

	public override void Exit()
	{
		baseCellObject?.CurrentCell?.cellFlow.fsm.SetTrigger(FSM.deselect);

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
	}

	public override void HoverUnpeek()
	{
		Debog.logGameflow("Hover unpeeking : " + gameObject.name);
		baseCellObject?.CurrentCell?.cellFlow.fsm.SetTrigger(FSM.unclickable);
	}
}
