using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGridDisplay : MonoBehaviour
{
	private void OnEnable()
	{
		FlowController.OnFlowPeekedGlobal += HandleHoverGlobal;
		FlowController.OnFlowUnpeekedGlobal += HandleUnhoverGlobal;

		FlowController.OnFlowEnteredGlobal += HandleEnterGlobal;
		FlowController.OnFlowExitedGlobal += HandleExitGlobal;

		//CellObjFlowController.OnHoverPeeked += HandleHover;
		//CellObjFlowController.OnHoverUnpeeked += HandleUnhover;

		//CellObjFlowController.OnEntered += HandleEnter;
		//CellObjFlowController.OnExited += HandleExit;
	}

	private void OnDisable()
	{
		FlowController.OnFlowPeekedGlobal -= HandleHoverGlobal;
		FlowController.OnFlowUnpeekedGlobal -= HandleUnhoverGlobal;

		FlowController.OnFlowEnteredGlobal -= HandleEnterGlobal;
		FlowController.OnFlowExitedGlobal -= HandleExitGlobal;

		//CellObjFlowController.OnHoverPeeked -= HandleHover;
		//CellObjFlowController.OnHoverUnpeeked -= HandleUnhover;

		//CellObjFlowController.OnEntered -= HandleEnter;
		//CellObjFlowController.OnExited -= HandleExit;
	}



	private void HandleEnterGlobal(FlowController flow)
	{
		CellObjFlowController cellObjFlow = flow as CellObjFlowController;
		if (cellObjFlow == null)
			return;

		cellObjFlow.cellObject.currCell.visuals.SetTrigger(CellState.select);
	}

	private void HandleExitGlobal(FlowController flow)
	{
		CellObjFlowController cellObjFlow = flow as CellObjFlowController;
		if (cellObjFlow == null)
			return;

		cellObjFlow.cellObject.currCell.visuals.UnsetTrigger(CellState.select);
	}
	
	private void HandleHoverGlobal(FlowController flow)
	{
		//if(flow ==)

		CellFlowController cellFlow = flow as CellFlowController;
		if (cellFlow != null)
		{
			cellFlow.cell.visuals.SetTrigger(CellState.hover);
			return;
		}

		if(flow is CellObjFlowController)
		{
			(flow as CellObjFlowController).cellObject.currCell.visuals.SetTrigger(
				flow.IsEnterable ? CellState.clickable : CellState.hover
				);
		}
	}
	private void HandleUnhoverGlobal(FlowController flow)
	{
		if (flow is CellFlowController)
		{
			(flow as CellFlowController).cell.visuals.UnsetTrigger(CellState.hover);
			return;
		}

		if (flow is CellObjFlowController)
		{
			(flow as CellObjFlowController).cellObject.currCell.visuals.UnsetTrigger(
				flow.IsEnterable ? CellState.clickable : CellState.hover
				);
		}
	}


	//private void HandleEnter(CellObjFlowController flow)
	//{
	//	flow.cellObject.currCell.visuals.SetTrigger(CellState.select);
	//}

	//private void HandleExit(CellObjFlowController flow)
	//{
	//	flow.cellObject.currCell.visuals.UnsetTrigger(CellState.select);
	//}

	//private void HandleHover(CellObjFlowController flow)
	//{
	//	flow.cellObject.currCell.visuals.SetTrigger(
	//		flow.IsEnterable ? CellState.clickable : CellState.hover
	//		);
	//}

	//private void HandleUnhover(CellObjFlowController flow)
	//{
	//	flow.cellObject.currCell.visuals.UnsetTrigger(
	//		flow.IsEnterable ? CellState.clickable : CellState.hover
	//		);
	//}
}
