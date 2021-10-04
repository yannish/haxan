using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WandererFlowController : CharacterFlowController, ICellCommandDispenser
{
	public CellCommand GetCellCommand(Cell cell) { return new CellPathCommand(cell); }

	[ReadOnly] public Wanderer wanderer;

	Stack<CharacterCommand> inputCommandStack;


	protected override void Awake()
	{
		base.Awake();
		wanderer = GetComponent<Wanderer>();
	}

	public override void Enter()
	{
		base.Enter();
		Globals.SelectedWanderer.Add(wanderer);
	}

	public override void Exit()
	{
		base.Exit();
		Globals.SelectedWanderer.Remove(wanderer);

		if(pathControl != null)
			pathControl.Discard();
	}

	public bool Tick()
	{
		if (Input.GetKeyDown(KeyCode.J))
			Debug.Log("keystroke in wanderer flow");

		return !inputCommandStack.IsNullOrEmpty();
	}

	public override bool TryGetCommandStack(ref Stack<CharacterCommand> commandStack)
	{
		//commandStack = null;

		if(!inputCommandStack.IsNullOrEmpty())
		{
			//var tempCommandStack = 
			commandStack = inputCommandStack;
			inputCommandStack = null;
			return true;
		}

		//commandStack = null;

		return false;
	}

	private ControlLens pathControl;
	public override bool HandleHover(ElementHoveredEvent e)
	{
		if (pathControl != null)
		{
			pathControl.Discard();
			pathControl = null;
		}

		if (e.element == null)
			return true;

		string hoveredThing = "";
		if (e.element != null)
			hoveredThing = e.element.gameObject.name;

		Debog.logGameflow("handling hover in wanderer flow: " + hoveredThing);


		if (
            //e.element?.flowController
			e.element.flowController != null
			//&& e.element.flowController != null
			&& e.element.flowController is CellFlowController
			&& (e.element.flowController as CellFlowController).cell != null
			)
		{
			var targetCell = (e.element.flowController as CellFlowController).cell;
			var pathedCells = Pathfinder.GetPath(wanderer.CurrentCell, targetCell);

            if (pathedCells.IsNullOrEmpty())
                return false;

			pathControl = CellActions.EffectCells(pathedCells, this);

			return true;
		}

		Debog.logGameflow("... wanderer flow didn't need hover: " + gameObject.name);

		return false;
	}

	public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		if (e.element.flowController is CellFlowController)
		{
			Debug.Log("clicked a cell in wanderer flow");

			var clickedCell = (e.element.flowController as CellFlowController).cell;

			if(
				clickedCell != wanderer.CurrentCell
				&& clickedCell.IsPassable
				)
			{
				var pathToCell = Pathfinder.GetPath(wanderer.CurrentCell, clickedCell);
				if (pathToCell == null)
					return FlowState.DONE;

				Stack<CharacterCommand>  newCommandStack = new Stack<CharacterCommand>();

				for (int i = pathToCell.Count - 1; i >= 0; i--)
				{
					Cell fromCell = i == 0 ? wanderer.CurrentCell : pathToCell[i - 1];

					var newStepCommand = new StepCommand(wanderer, fromCell, pathToCell[i], 1f);
					newCommandStack.Push(newStepCommand);

					Debug.Log("pushing step command " + newStepCommand.ToString());
				}

				inputCommandStack = newCommandStack;

				wanderer.CurrentCell?.cellFlow.fsm.SetTrigger("unselect");
				clickedCell.cellFlow?.fsm.SetTrigger("select");

				return FlowState.RUNNING;
			}
		}

		return FlowState.YIELD;
	}
}
