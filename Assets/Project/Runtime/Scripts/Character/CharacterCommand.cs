using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class CharacterCommand
{
	public float currTime;
	public float currProgress;
	public float duration = 1f;


	public CharacterCommand(CharacterFlow characterFlow)
	{
		this.characterFlow = characterFlow;
	}

	public CharacterFlow characterFlow;

	public virtual void Peek() { }

	public virtual void Unpeek() { }

	public virtual void Start() { }

	public virtual void End() { }

	//... ticks true when the command is complete
	public virtual bool Tick()
	{
		if (duration == 0f)
			return false;

		currTime += Time.deltaTime;
		currProgress = Mathf.Clamp01(currTime / duration);

		return currProgress >= 1f;
	}
}


//public abstract class CharacterCommand
//{
//	public Character character;

//    public CharacterCommand(Character character)
//	{
//		this.character = character;
//	}

//	public abstract bool IsValid();

//	public float duration;

//	public float currTime;

//	public float currProgress;


//	/*		
//	 *		
//	 	these are for smashing state to its final form:
//		wouldn't just run Execute on a command if it's playing out over time though
//		moving is really only "executed" once the character arrives at & is bound to
//		its destination cell.

//	*/

//	public virtual void Peek() { }

//	public virtual void Unpeek() { }

//	public abstract void Execute();
//	public abstract void Undo();

//	public virtual void Begin() { currTime = duration; }
//	public virtual void End() { currTime = duration; }

//	public virtual bool Tick() { return currTime <= 0f; }
//}
