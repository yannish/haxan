using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellCommand
{

	public Cell cell;
	//public CellCommand(Cell cell)
	//{
	//	this.cell = cell;
	//}

	public abstract void Execute();
	
	public abstract void Undo();
}

public class CellHoverCommand : CellCommand
{
	public CellHoverCommand() { }

	public override void Execute()
	{
		cell.cellFlow.fsm.SetTrigger(FSM.hover);
		//cell.baseFlow.animator.SetBool("hover", true);
    }

	public override void Undo()
	{
		cell.cellFlow.fsm.SetTrigger(FSM.unhover);
		//cell.baseFlow.animator.SetBool("hover", false);
    }
}

public class CellPathCommand : CellCommand
{
	public CellPathCommand() { }

	public override void Execute()
	{
		cell.cellFlow.fsm.SetTrigger(FSM.path);
		//cell.baseFlow.animator.SetBool("path", true);
    }

	public override void Undo()
	{
		cell.cellFlow.fsm.SetTrigger(FSM.unpath);
		//cell.baseFlow.animator.SetBool("path", false);
    }
}