using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepCommand : CharacterCommand
{
	public Cell targetCell;
	public Cell fromCell;

	public Vector3 startPos;
	public Vector3 endPos;

	public StepCommand(
		CharacterFlow characterFlow,
		Cell fromCell,
		Cell targetCell,
		float duration
		) : base(characterFlow)
	{
		this.targetCell = targetCell;
		this.fromCell = fromCell;
		this.duration = duration;

		startPos = fromCell.occupantPivot.position;
		endPos = targetCell.occupantPivot.position;
	}

	public override void OnBeginTick()
	{
		base.OnBeginTick();

		string log = string.Format("executing from : {0} to : {1}", fromCell.name, targetCell.name);
		Debog.logGameflow(log);
		fromCell.Leave(characterFlow.character);
	}

	public override void OnCompleteTick()
	{
		base.OnCompleteTick();
		targetCell.Enter(characterFlow.character);
	}

	public override void Execute()
	{
		base.Execute();
		characterFlow.character.currMove--;
		characterFlow.character.SetVisualPos(Vector3.zero, true);
		characterFlow.character.MoveAndBindTo(targetCell);
	}

	public override void Undo()
	{
		base.Undo();
		characterFlow.character.currMove++;
		characterFlow.character.MoveAndBindTo(fromCell);
		characterFlow.character.SetVisualPos(Vector3.zero, true);
	}

	public override bool Tick()
	{
		currTime += Time.deltaTime;
		currProgress = Mathf.Clamp01(currTime / duration);

		characterFlow.character.SetVisualPos(Vector3.Lerp(startPos, endPos, currProgress));

		return currProgress >= 1f;
	}
}

//public class StepCommand : CharacterCommand
//{
//	public StepCommand(
//		Character character, 
//		Cell fromCell, 
//		Cell targetCell,
//		float duration
//		) : base(character)
//	{
//		this.targetCell = targetCell;
//		this.fromCell = fromCell;
//		this.duration = duration;
//	}

//	private Cell targetCell;
//	private Cell fromCell;

//	public override bool IsValid()
//	{
//		if (character == null || targetCell == null)
//			return false;

//		//... this feels a bit clunky, getting at the .cell through the flow.
//		/*
//		 * what's rationale for always passing around FlowControllers.... is it good...?
//		 * what else would be the thing
//		 * previously it was UIElements, and then cells were UIElements
//		 */

//		if (targetCell == null || !character.IsBound())
//			return false;

//		return targetCell.IsPassable;
//	}

//	public override void Begin()
//	{
//		base.Begin();
//	}





//	public override void Undo()
//	{
//		Debug.Log("undoing " + this.ToString());

//		character.MoveAndBindTo(fromCell);
//	}

//	public override string ToString()
//	{
//		return character.name + ", moving FROM: " + fromCell.name + ", TO: " + targetCell.name;
//	}

//	public override void Peek()
//	{
//		throw new System.NotImplementedException();
//	}
//}
