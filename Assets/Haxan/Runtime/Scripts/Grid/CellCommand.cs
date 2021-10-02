using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellCommand
{
	public Cell cell;
	public CellCommand(Cell cell)
	{
		this.cell = cell;
	}

	public abstract void Execute();
	//public abstract void Execute(Cell cell);
	public abstract void Undo();
	//public abstract void Undo(Cell cell);
}

public class CellHoverCommand : CellCommand
{
	public CellHoverCommand(Cell cell) : base(cell)
	{
	}

	public override void Execute()
	{
		cell.cellFlow.fsm.SetTrigger("hover");
		//cell.baseFlow.animator.SetBool("hover", true);
    }

	public override void Undo()
	{
		cell.cellFlow.fsm.SetTrigger("unhover");
		//cell.baseFlow.animator.SetBool("hover", false);
    }
}

public class CellPathCommand : CellCommand
{
	public CellPathCommand(Cell cell) : base(cell) { }

	public override void Execute()
	{
		cell.cellFlow.fsm.SetTrigger("path");
		//cell.baseFlow.animator.SetBool("path", true);
    }

	public override void Undo()
	{
		cell.cellFlow.fsm.SetTrigger("unpath");
		//cell.baseFlow.animator.SetBool("path", false);
    }
}