using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BindingMapExtensions
{
	public static bool TryGetBoundCell(this CellObject cellObj, out Cell_OLD cell)
	{
		cell = default(Cell_OLD);
		return Globals.Grid.cellObjectBindings.TryGetBinding(cellObj, out cell);
	}

	public static bool TryGetBoundCellObject(this Cell_OLD cell, out CellObject cellObject)
	{
		cellObject = default(CellObject);
		return Globals.Grid.cellObjectBindings.TryGetBinding(cell, out cellObject);
	}

	public static bool IsBound(this CellObject cellObject)
	{
		if (Globals.Grid == null)
			return false;

		return Globals.Grid.cellObjectBindings.CheckIsBound(cellObject);
	}

	public static bool IsBound(this Cell_OLD cell)
	{
		if (Globals.Grid == null)
			return false;

		return Globals.Grid.cellObjectBindings.CheckIsBound(cell);
	}

	public static bool Unbind(this Cell_OLD cell)
	{
		if (cell.IsBound())
		{
			Globals.Grid.cellObjectBindings.Unbind(cell);
			return true;
		}

		return false;
	}

	public static bool Unbind(this CellObject cellObject)
	{
		if (cellObject.IsBound())
		{
			Globals.Grid.cellObjectBindings.Unbind(cellObject);
			return true;
		}

		return false;
	}

	public static bool Bind(this CellObject cellObject, Cell_OLD cell)
	{
		return DoBind(cellObject, cell);
	}

	public static bool Bind(this Cell_OLD cell, CellObject cellObject)
	{
		return DoBind(cellObject, cell);
	}

	public static void BindInPlace(this CellObject cellObj)
	{
		Cell_OLD foundCell = cellObj.NearestCell();

		if (foundCell)
			Bind(foundCell, cellObj);
		else
			Debug.LogWarning(cellObj.name + " wasn't able to bind in place", cellObj);
	}

	private static bool DoBind(CellObject cellObject, Cell_OLD cell)
	{
		if (cellObject.IsBound() || cell.IsBound())
		{
			if(cell.TryGetBoundCellObject(out var foundObject))
			{
				if(foundObject != cellObject)
				{
					Debug.LogWarning(
						"... wasn't able to bind " + cellObject.name + 
						" in place, cell was already bound to " + foundObject.name, 
						cell
						);
				}

				return false;
			}
		}

		Globals.Grid.cellObjectBindings.Bind(cell, cellObject);
		return true;
	}
}
