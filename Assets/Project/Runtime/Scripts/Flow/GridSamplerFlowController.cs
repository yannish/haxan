using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSamplerFlowController : FlowController
{
	public override void Enter()
	{
		Debug.Log("Entered Grid Sampler flow");
	}

	public override void Exit()
	{
		base.Exit();
		Debug.Log("Entered Grid Sampler flow");
	}

	public override bool HandleHoverStart(ElementHoveredEvent e)
	{
		if (e.element == null)
			return false;

		Cell hoveredHexCell = e.element.GetComponent<Cell>();
		if (hoveredHexCell)
		{
			Debug.Log("Hovered a hexCell: " + hoveredHexCell.coords.ToString());
			return true;
		}

		return false;
	}

	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		//if(e.element is HexCell)
		//{

		//}
		return FlowState.YIELD;
	}
}
