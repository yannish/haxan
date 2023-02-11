using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CharacterCommand : CellObjectCommand
{
	public Character character;

	public CharacterCommand(Character character) : base (character)
	{
		this.cellObject = character;
		this.character = character;
	}
}

[Serializable]
public abstract class CellObjectCommand : CellObjectCommandBase<CellObject>
{
	public CellObjectCommand(CellObject cellObject) : base(cellObject)
	{
		this.cellObject = cellObject;
		//base(cellObject);
	}
}


[Serializable]
public abstract class CellObjectCommandBase<T> where T : CellObject
{
	public T cellObject;

	public float currTime;
	public float currProgress;
	public float duration = 1f;

	public CharacterFlowController characterFlow;

	public CellObjectCommandBase(T cellObject)
	{
		this.cellObject = cellObject;
	}

	//public CellObjectCommand(CharacterFlowController characterFlow)
	//{
	//	this.characterFlow = characterFlow;
	//}

	public virtual void DrawInspectorContent() { }

	public virtual void Peek() { }

	public virtual void Unpeek() { }

	public virtual void OnBeginTick() { }

	public virtual void OnCompleteTick() { }

	public virtual void OnBeginReverseTick() { }

	public virtual void OnCompleteReverseTick() { }

	public virtual void Execute() { }

	public virtual void Undo() { }

	public virtual bool CheckComplete(float scale = 1f) => Mathf.Sign(scale) > 0f ? currProgress >= 1f : currProgress <= 0f;


	//... ticks true when the command is complete
	public virtual bool Tick(float timeScale = 1f)
	{
		if (duration == 0f)
			return false;

		currTime += Time.deltaTime * timeScale;
		currProgress = Mathf.Clamp01(currTime / duration);

		return CheckComplete(timeScale);
	}
	
	//public virtual void IncurCost() { }
}