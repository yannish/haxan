using BOG;


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFlowController : CellObjFlowController
{
	[ReadOnly] public CellVisuals visuals;
    [ReadOnly] public QuickStateMachine fsm;
    [ReadOnly] public Cell cell;

    protected override void Awake()
	{
		visuals = GetComponentInChildren<CellVisuals>();
        fsm = GetComponentInChildren<QuickStateMachine>();
		cell = GetComponent<Cell>();
    }

	//public override void HoverPeek() => visuals?.SetTrigger(FSMtrigger.hover, true);
	//public override void HoverPeek() => fsm?.SetTrigger(FSM.hover);

	//public override void HoverUnpeek() => visuals?.SetTrigger(FSMtrigger.hover, false);
	//public override void HoverUnpeek() => fsm?.SetTrigger(FSM.unhover);

	//if(baseCellObject)
	//	fsm.SetTrigger("clickable");
	//baseCellObject?.CurrentCell?.cellFlow.fsm.SetTrigger("clickable");

	//if (baseCellObject)
	//	fsm.SetTrigger("unclickable");
	//baseCellObject?.CurrentCell?.cellFlow.fsm.SetTrigger("unclickable");

	//public override void Enter() { fsm?.SetTrigger("select"); }
	//public override void Exit() { fsm?.SetTrigger("unselect"); }

	//public override void HoverPeek() { animator?.SetBool("hover", true); }
	//public override void HoverUnpeek() { animator?.SetBool("hover", false); }

	//public override void Enter() { animator?.SetBool("select", true); }
	//public override void Exit() { animator?.SetBool("select", false); }
}
