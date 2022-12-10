using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellCommand
{
	public Cell cell;

	public abstract CellState triggerType { get; }

	public virtual void Execute() => cell.visuals.SetTrigger(triggerType);
	public virtual void Undo() => cell.visuals.UnsetTrigger(triggerType);

	//public virtual void Execute() => cell.cellFlow.fsm.SetTrigger(trigger);
	//public virtual void Undo() => cell.cellFlow.fsm.SetTrigger(untrigger);


}

public class CellHoverCommand : CellCommand
{
	public CellHoverCommand() { }
	public override CellState triggerType => CellState.hover;

	//public override string trigger => FSM.hover;
	//public override string untrigger => FSM.unhover;
}

public class CellClickableCommand : CellCommand
{
	public CellClickableCommand() { }
	public override CellState triggerType => CellState.clickable;

	//public override string trigger => FSM.clickable;
	//public override string untrigger => FSM.unclickable;
}

public class CellPeekClickableCommand : CellCommand
{
	public CellPeekClickableCommand() { }
	public override CellState triggerType => CellState.clickable;

	//public override string trigger => FSM.peekClickable;
	//public override string untrigger => FSM.unpeekClickable;
}

public class CellPathCommand : CellCommand
{
	public CellPathCommand() { }
	public override CellState triggerType => CellState.pathShown;

	//public override string trigger => FSM.path;
	//public override string untrigger => FSM.unpath;
}

public class CellPeekPathCommand : CellCommand
{
	public CellPeekPathCommand() { }
	public override CellState triggerType => CellState.pathHint;

	//public override string trigger => FSM.peekPath;
	//public override string untrigger => FSM.unpeekPath;
}