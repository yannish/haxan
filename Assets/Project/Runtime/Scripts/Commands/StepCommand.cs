using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class StepCommand : CharacterCommand
{
	public Cell targetCell;
	public Cell fromCell;

	public Vector3 startPos;
	public Vector3 endPos;


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

		startPos = fromCell.occupantPivot.position;
		endPos = targetCell.occupantPivot.position;
	}


	public override void OnBeginTick()
	{
		base.OnBeginTick();
		fromCell.Leave(character);
		//string log = string.Format("executing from : {0} to : {1}", fromCell.name, targetCell.name);
		//Debog.logGameflow(log);
	}

	public override void OnBeginReverseTick()
	{
		base.OnBeginTick();
		targetCell.Leave(character);
		//string log = string.Format("executing from : {0} to : {1}", fromCell.name, targetCell.name);
		//Debog.logGameflow(log);
	}


	public override void OnCompleteTick()
	{
		base.OnCompleteTick();
		targetCell.Enter(character);
	}

	public override void OnCompleteReverseTick()
	{
		base.OnCompleteTick();
		fromCell.Enter(character);
	}


	public override void Execute()
	{
		base.Execute();
		character.DecrementMove();
		character.SetVisualPos(Vector3.zero, true);
		character.MoveAndBindTo(targetCell);
	}

	public override void Undo()
	{
		base.Undo();
		character.IncrementMove();
		character.MoveAndBindTo(fromCell);
		character.SetVisualPos(Vector3.zero, true);
	}

	public override bool Tick(float timeScale = 1f)
	{
		base.Tick(timeScale);

		character.SetVisualPos(Vector3.Lerp(startPos, endPos, currProgress));

		return CheckComplete(timeScale);
	}

#if UNITY_EDITOR
	public override void DrawInspectorContent()
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.ObjectField("from:", fromCell, typeof(Cell), true);
		EditorGUILayout.ObjectField("to:", targetCell, typeof(Cell), true);
		EditorGUILayout.EndHorizontal();
	}
#endif
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
