using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepCommand : CharacterCommand
{
	public StepCommand(
		Character character, 
		Cell fromCell, 
		Cell targetCell,
		float duration
		) : base(character)
	{
		this.targetCell = targetCell;
		this.fromCell = fromCell;
		this.duration = duration;
	}

	private Cell targetCell;
	private Cell fromCell;

	public override bool IsValid()
	{
		if (character == null || targetCell == null)
			return false;

		//... this feels a bit clunky, getting at the .cell through the flow.
		/*
		 * what's rationale for always passing around FlowControllers.... is it good...?
		 * what else would be the thing
		 * previously it was UIElements, and then cells were UIElements
		 */
		
		if (targetCell == null || !character.IsBound())
			return false;

		return targetCell.IsPassable;
	}

	public override void Begin()
	{
		base.Begin();
	}

	public override bool Tick()
	{
		currTime += Time.deltaTime;
		currProgress = Mathf.Clamp01(currTime / duration);

		return base.Tick();
	}

	public override void Execute()
	{
		Debug.Log("executing STEP for " + this.ToString());

		//if (!character.TryGetBoundCell(out fromCell))
		//	Debug.Log("failed to grab 'from' cell for step command");

		fromCell.Leave(character);
		character.MoveAndBindTo(targetCell);
	}

	public override void Undo()
	{
		Debug.Log("undoing " + this.ToString());

		character.MoveAndBindTo(fromCell);
	}

	public override string ToString()
	{
		return character.name + ", moving FROM: " + fromCell.name + ", TO: " + targetCell.name;
	}

	public override void Peek()
	{
		throw new System.NotImplementedException();
	}
}
