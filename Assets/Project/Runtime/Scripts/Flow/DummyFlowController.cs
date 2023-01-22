using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyFlowController : CellObjFlowController
{
	public ScrObjAbility ability;

	private Action pathControl;
	private Action markupControl;


	[ReadOnly] public DummyCube dummyCube;
	[ReadOnly] public QuickStateMachine fsm;

	protected override void Awake()
	{
		base.Awake();

		fsm = GetComponentInChildren<QuickStateMachine>();
		dummyCube = GetComponent<DummyCube>();
	}

	public override void HoverPeek()
	{
		fsm?.SetTrigger(FSM.hover);
		base.HoverPeek();
	}

	public override void HoverUnpeek()
	{
		fsm?.SetTrigger(FSM.unhover);
		base.HoverUnpeek();
	}

	public override void Enter()
	{
		base.Enter();

		fsm.SetTrigger(FSM.select);

		pathControl = CellActions.EffectCells<CellPathCommand>(
			dummyCube.currCell.GetCardinalRing(2, t => !t.IsBound())
			);
	}

	public override void Exit()
	{
		base.Exit();

		fsm.SetTrigger(FSM.deselect);

		if (pathControl != null)
		{
			pathControl.Invoke();
			pathControl = null;
		}

		if(markupControl != null)
		{
			markupControl.Invoke();
			markupControl = null;
		}
	}

	public override bool HandleHoverStart(ElementHoveredEvent e)
	{
		if(pathControl != null)
		{
			pathControl.Invoke();
			pathControl = null;
		}

		if (markupControl != null)
		{
			markupControl.Invoke();
			markupControl = null;
		}

		if (
			e.element == null
			|| e.element.flowController == null
			|| !(e.element.flowController is CellFlowController)
			|| (e.element.flowController as CellFlowController).cell == null
			)
			return false;

		Cell hoveredCell = (e.element.flowController as CellFlowController).cell;
		var pathedCells = Pathfinder.GetPath(dummyCube.currCell, hoveredCell);

		if (pathedCells.IsNullOrEmpty())
			return false;

		//pathControl = CellActions.EffectCells<CellPathCommand>(pathedCells);

		markupControl = CellMarkupService.I.MarkMovePath(dummyCube.currCell, pathedCells);

		return false;
	}

	//public override void HoverPeek()
	//{
	//	//Debug.Log("Hover peeking dummyFLow");
	//	dummyCube.CurrentCell?.baseFlow.animator.SetBool("clickable", true);
	//}

	//public override void HoverUnpeek()
	//{
	//	//Debug.Log("Hover unpeeking dummyFLow");
	//	dummyCube.CurrentCell?.baseFlow.animator.SetBool("clickable", false);
	//}

	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		return FlowState.YIELD;
	}
}
