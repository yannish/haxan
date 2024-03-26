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

	public AnimationClip[] clips;
	public AnimationCurve[] curves;

	public OpPlaybackData(Unit unit, float startTime, float duration)
	{
		this.unitIndex = unit.ToIndex();
		this.startTime = startTime;
		this.duration = duration;
		this.clips = new AnimationClip[0];
		this.curves = new AnimationCurve[0];
	}
}

public static class OpDataExtensions
{
	public static void DrawPlaybackData(this OpPlaybackData data)
	{
#if UNITY_EDITOR
		//var foundUnit = data.unitIndex.ToUnit();
		//if (foundUnit != null)
		//	EditorGUILayout.ObjectField(
		//		$"UNIT INDEX: {data.unitIndex}",
		//		data.unitIndex.ToUnit(),
		//		typeof(Unit),
		//		true
		//		);
		//else
		//	EditorGUILayout.LabelField($"... can't find unit at {data.unitIndex}");
		using (new GUILayout.HorizontalScope())
		{
			EditorGUILayout.LabelField($"START TIME : {data.startTime}", GUILayout.MaxWidth(180f));
			EditorGUILayout.LabelField($"DURATION : {data.duration}", GUILayout.MaxWidth(180f));
			EditorGUILayout.LabelField($"END TIME : {data.endTime}", GUILayout.MaxWidth(180f));
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
public abstract class UnitOp
{
	public UnitOp(Unit unit)
	{
		this.unit = unit;
		this.unitIndex = unit.ToIndex();
	}

	public Unit unit;
	public int unitIndex;
	public OpPlaybackData playbackData;

	public float startTime;
	public float duration;

	public abstract void Execute(Unit unit);
	public abstract void Undo(Unit unit);
	public abstract void Tick(Unit unit, float t);

	public virtual void OnBeginTick() { }
	public virtual void OnCompleteTick() { }

	public virtual void OnBeginReverseTick() { }
	public virtual void OnCompleteReverseTick() { }

	public virtual void DrawInspectorContent() { }
}


