using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellInfoDisplay : MonoBehaviour
{
	//public static event Action OnCellUnhovered = delegate { };

	public CellDisplaySlot[] slots;

	private void Awake()
	{
		//Events.instance.AddListener<ElementHoveredEvent>(CellHovered);
		slots = GetComponentsInChildren<CellDisplaySlot>();

		Cell_OLD.OnCellHovered += DisplayCell;
		Cell_OLD.OnCellUnhovered += Clear;

		Clear();
	}

	//void CellHovered(Cell cell)
	void CellHovered(ElementHoveredEvent e)
	{
		Clear();

		Debug.Log("element hovered for cell display: ");

		if (
			e.element != null 
			&& e.element.flowController != null
			&& e.element.flowController is CellFlowController
			)
			DisplayCell((e.element.flowController as CellFlowController).cell);
	}

	private void DisplayCell(Cell_OLD cell)
	{
		//Debug.Log("... displaying cell ");

		int j = 0;
		for (int i = 0; i < cell.preset.configs.Count; i++)
		{
			if (cell.preset.configs[i].icon != null)
			{
				slots[i].DisplayCellConfig(cell.preset.configs[i]);
				j++;
			}
		}
	}

	private void Clear()
	{
		//Debug.Log("... clearing cell ");

		foreach (var slot in slots)
			slot.Clear();
	}
}
