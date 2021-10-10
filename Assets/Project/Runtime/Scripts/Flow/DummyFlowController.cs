using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyFlowController : FlowController
{
	public ScrObjAbility ability;

	[ReadOnly] public DummyCube dummyCube;
	[ReadOnly] public ControlLens clickedLens;
	[ReadOnly] public QuickStateMachine fsm;
	//Animator animator;

	protected override void Awake()
	{
		base.Awake();
		//animator = GetComponent<Animator>();
		fsm = GetComponentInChildren<QuickStateMachine>();
		dummyCube = GetComponent<DummyCube>();
		//uiElement = GetComponent<AnimatorUIElement>();
	}

	public override void HoverPeek()
	{
		Debug.Log("Hover peeking : " + gameObject.name);

		fsm?.SetTrigger(FSM.hover);

		base.HoverPeek();
	}

	public override void HoverUnpeek()
	{
		Debug.Log("Hover unpeeking : " + gameObject.name);

		fsm?.SetTrigger(FSM.unhover);

		base.HoverUnpeek();
	}

	public override void Enter()
	{
		base.Enter();

		//Debug.Log("Entered DummyFLow");

		fsm.SetTrigger(FSM.select);

		if (
			ability != null
			&& dummyCube.CurrentCell
			)
		{
			clickedLens = CellActions.EffectCells(
				ability.GetReachableCells(dummyCube.CurrentCell),
				ability as ICellCommandDispenser
				);
		}

		//dummyCube.CurrentCell?.baseFlow.animator.SetBool("select", true);

		//if (dummyCube.CurrentCell)
		//{
		//	clickedLens = new ControlLens();

		//	var effectedCells = dummyCube.CurrentCell.coords.GetCardinalLine(HexDirection.NE, 3);

		//	Debug.Log("Effected cells: " + effectedCells.Count);

		//	foreach (var cell in effectedCells)
		//	{
		//		CellPathCommand command = new CellPathCommand(cell);
		//		command.Execute();
		//		clickedLens.OnDiscard(() => command.Undo());
		//	}
		//}
	}

	public override void Exit()
	{
		base.Exit();
		//Debug.Log("Exited DummyFLow");
		//dummyCube.CurrentCell?.baseFlow.animator.SetBool("select", false);

		fsm.SetTrigger(FSM.deselect);

		if (clickedLens != null)
		{
			clickedLens.Discard();
			clickedLens = null;
		}
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
