using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICellCommandDispenser
{
	CellCommand GetCellCommand(Cell cell);
}

//public class HexCell : Cell
//{

//}

public static class CellActions
{
	public static void SetNeighbour(this Cell cell, Cell newNeighbour, HexDirection direction)
	{
		cell.neighbours[(int)direction] = newNeighbour;
		newNeighbour.neighbours[(int)direction.Opposite()] = cell;
	}

	public static Cell GetNeighbour(this Cell cell, HexDirection direction)
	{
		return cell.neighbours[(int)direction];
	}

	public static Action NewEffectCells<T>(List<Cell> cells) where T : CellCommand, new()
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

	public static ControlLens EffectCells(
		List<Cell> cells,
		ICellCommandDispenser commandDispenser
		)
	{
		ControlLens lens = ScriptableObject.CreateInstance<ControlLens>();
		//ControlLens lens = new ControlLens();

		foreach (var cell in cells)
		{
			var effectCommend = commandDispenser.GetCellCommand(cell);
			effectCommend.Execute();
			lens.OnPostDiscard(() => effectCommend.Undo());
		}

		return lens;
	}
}