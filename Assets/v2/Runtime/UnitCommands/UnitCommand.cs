using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCommand 
{
	public Unit unit;
	public float currTime;
	public float currProgress;
	public float duration = -1f;

	public virtual void DrawInspectorContent() { }

	public virtual void OnBeginTick() { }

	public virtual void OnCompleteTick() { }

	public virtual void OnBeginReverseTick() { }

	public virtual void OnCompleteReverseTick() { }

	public virtual void Execute() { }

	public virtual void Undo() { }

	public virtual bool Tick(float timeScale = 1f)
	{
		if (duration == 0f)
		{
			Debug.LogWarning("... this command's duration is 0?");
			return false;
		}

		currTime += Time.deltaTime * timeScale;
		currProgress = Mathf.Clamp01(currTime / duration);

		return CheckComplete(timeScale);
	}

	public virtual bool CheckComplete(float scale = 1f) => Mathf.Sign(scale) > 0f ? currProgress >= 1f : currProgress <= 0f;

	public virtual bool StepsTimeForward() => true;


}

//public class UnitCommandBase<T> where T : Unit
//{

//}