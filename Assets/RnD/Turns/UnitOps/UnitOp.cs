using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


public interface IUnitOperable
{
	public void Execute(Unit unit);

	public void Undo(Unit unit);

	public void Tick(Unit unit, float t);

	public void DrawInspectorContent();
}

/// <summary>
/// Ops must have targets, which are in the scene.
/// How to get a handle on these in a way that will survive serialization?
/// Have a big rebuildable runtime sets with all your units, and store
/// indices into those lists...?
/// </summary>
/// 

//... Does every Op affect one & only one unit?
//... Probably. 

[Serializable]
public abstract class UnitOp
{
	public UnitOp(Unit unit)
	{
		this.unit = unit;
		this.unitIndex = unit.toIndex();
	}

	public Unit unit;
	public int unitIndex;

	public float startTime;
	public float duration;

	public abstract void Execute(Unit unit);
	public abstract void Undo(Unit unit);
	public abstract void Tick(Unit unit, float t);

	public virtual void DrawInspectorContent() { }
}

