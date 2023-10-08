using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public interface ICellCommandDispenser
//{
//	CellCommand GetCellCommand(Cell cell);
//}


public static class CellActions
{
	public static bool HasPathTo(
		this Cell_OLD fromCell,
		Cell_OLD toCell,
		int maxLength = -1
		)
	{
		var foundPath = Pathfinder_OLD.GetPath(fromCell, toCell);
		if (foundPath.IsNullOrEmpty())
			return false;

		if (maxLength > 0)
			return foundPath.Count <= maxLength;

		return true;
	}

	public static bool HasPathTo(
		this Cell_OLD fromCell, 
		Cell_OLD toCell, 
		out List<Cell_OLD> foundPath,
		int maxLength = -1
		)
	{
		foundPath = Pathfinder_OLD.GetPath(fromCell, toCell);
		if (foundPath.IsNullOrEmpty())
			return false;

		if (maxLength > 0)
			return foundPath.Count <= maxLength;

		return true;
	}

	//public static bool HasPathTo

	public static void SetNeighbour(this Cell_OLD cell, Cell_OLD newNeighbour, HexDirection direction)
	{
		cell.neighbours[(int)direction] = newNeighbour; 
		newNeighbour.neighbours[(int)direction.Opposite()] = cell;
	}

	public static Cell_OLD GetNeighbour(this Cell_OLD cell, HexDirection direction)
	{
		return cell.neighbours[(int)direction];
	}

	public static Action EffectCells<T>(List<Cell_OLD> cells) where T : CellCommand, new()
	{
		Action undo = () => { };

		foreach (var cell in cells)
		{
			T newCommand = new T();
			newCommand.cell = cell;
			newCommand.Execute();
			undo += () => newCommand.Undo();
		}

		return undo;
	}

	//public static ControlLens EffectCells(
	//	List<Cell> cells,
	//	ICellCommandDispenser commandDispenser
	//	)
	//{
	//	ControlLens lens = ScriptableObject.CreateInstance<ControlLens>();
	//	//ControlLens lens = new ControlLens();

	//	foreach (var cell in cells)
	//	{
	//		var effectCommend = commandDispenser.GetCellCommand(cell);
	//		effectCommend.Execute();
	//		lens.OnPostDiscard(() => effectCommend.Undo());
	//	}

	//	return lens;
	//}
}