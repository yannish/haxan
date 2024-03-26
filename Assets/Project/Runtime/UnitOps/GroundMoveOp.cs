using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class HaxanClip
{
	public AnimationClip clip;
	public AnimationCurve curve;
	public float speed;
	public float weight;
}

[Serializable]
public class GroundMoveOp : UnitOp
{
	public GroundedMove groundMoveAbility;

	public Vector2Int fromCoord;
	public Vector2Int toCoord;

	public Vector3 startPos;
	public Vector3 endPos;

	[Header("ANIMATION:")]
	public List<AnimationClip> clips = new List<AnimationClip>();
	public List<AnimationCurve> blendCurves = new List<AnimationCurve>();
	List<ClipHandle> clipHandles = new List<ClipHandle>();

	public override void DrawInspectorContent()
	{
#if UNITY_EDITOR
		EditorGUILayout.LabelField(
			$"MOVE : {fromCoord.ToCoordString()} => {toCoord.ToCoordString()}",
			EditorStyles.boldLabel
			);
			
		playbackData.DrawPlaybackData();
#endif
	}

	public GroundMoveOp(
		Unit unit, 
		Vector2Int fromCoord, 
		Vector2Int toCoord,
		float startTime,
		float duration
		) : base(unit)
	{
		this.fromCoord = fromCoord;
		this.toCoord = toCoord;

		startPos = Board.OffsetToWorld(fromCoord);
		endPos = Board.OffsetToWorld(toCoord);

		this.playbackData = new OpPlaybackData(unit, startTime, duration);
	}

	public override void Execute(Unit unit)
	{
		unit.DecrementMove();
		unit.SetVisualPos(Vector3.zero, true);
		unit.MoveTo(toCoord);
	}

	public override void Undo(Unit unit)
	{
		unit.IncrementMove();
		unit.SetVisualPos(Vector3.zero, true);
		unit.MoveTo(fromCoord);
	}

	public override void Tick(Unit unit, float t)
	{
		unit.SetVisualPos(Vector3.Lerp(startPos, endPos, t));

		for (int i = 0; i < clipHandles.Count; i++)
		{
			var clipHandle = clipHandles[i];
			var blendCurve = playbackData.curves[i];

			clipHandle.clipPlayable.SetTime(t);
			clipHandle.inputWeight = blendCurve.Evaluate(t);

			//clipHandle.clipPlayable.SetInputWeight(0, blendCurve.Evaluate(t));
		}
	}

	//TODO: NOT called on first move?
	public override void OnBeginTick()
	{
		Debug.LogWarning("BEGIN GROUND MOVE TICK");
		foreach(var clip in playbackData.clips)
		{
			Debug.LogWarning("... GRABBING SCRUB CLIP");
			var newClipHandle = unit.clipPlayer.GetScrubClip(clip, 0f);
			clipHandles.Add(newClipHandle);
		}
	}

	//TODO:
    //  probably not the place to "release" these clips. animation will end up blank in between ops,
    //  especially the last in a sequence?
	public override void OnCompleteTick()
	{
		Debug.LogWarning("COMPLETE GROUND MOVE TICK");

        foreach (var clip in clipHandles)
		{
			unit.clipPlayer.ReleaseScrubClip(clip);
		}	
        clipHandles.Clear();

        //for (int i = clipHandles.Count - 1; i >= 0; i--)
        //{
        //    unit.clipPlayer.ReleaseScrubClip(clip);
        //}
	}
}