using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*	Do unitOps need to worry about startTime / duration / etc, all details around playback.
 *	If they don't, something does. Another array of somethings. OpDatas.
 *	
 *	Let's say yes for now.
 */

[Serializable]
public struct OpPlaybackData
{
	public int unitIndex;
	public float startTime;
	public float duration;
	public float endTime { get => startTime + duration; }

	public OpPlaybackData(Unit unit, float startTime, float duration)
	{
		this.unitIndex = unit.ToIndex();
		this.startTime = startTime;
		this.duration = duration;
	}
}

public static class OpDataExtensions
{
	public static void DrawOpData(this OpPlaybackData data)
	{
#if UNITY_EDITOR
		EditorGUILayout.ObjectField($"UNIT INDEX: {data.unitIndex}", data.unitIndex.ToUnit(), typeof(Unit), true);
		using (new GUILayout.HorizontalScope())
		{
			EditorGUILayout.LabelField($"start time: {data.startTime}", GUILayout.MaxWidth(80f));
			EditorGUILayout.LabelField($"duration: {data.duration}", GUILayout.MaxWidth(80f));
		}
#endif
	}
}

public interface IUnitOperable
{
	public OpPlaybackData data { get; }

	//public void OnBeginTick(Unit unit);

	public void Execute(Unit unit);

	//public void OnCompleteTick(Unit unit);

	//public void OnBeginReverseTick(Unit unit);

	public void Undo(Unit unit);

	//public void OnCompleteReverseTick(Unit unit);

	//... what kind of time are we feeding in here.. what is simplest... "normalized"?
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
public abstract class UnitOpClass
{
	public UnitOpClass(Unit unit)
	{
		this.unit = unit;
		this.unitIndex = unit.ToIndex();
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

