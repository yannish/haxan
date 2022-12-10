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

	protected override void Awake() => cellObject = GetComponent<CellObject>();


	public override void Enter()
	{
		base.Enter();

		if (cellObject != null)
			OnEntered(this);
	}

	public override void Exit()
	{
		base.Exit();

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

		if (cellObject != null)
			OnExited(this);
	}
}
