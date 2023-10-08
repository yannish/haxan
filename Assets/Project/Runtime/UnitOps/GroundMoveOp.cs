using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class GroundMoveOp : IUnitOperable
{
	public Vector2Int fromCoord;
	public Vector2Int toCoord;

	public Vector3 startPos;
	public Vector3 endPos;

	public OpData _data;
	public OpData data => _data;

	public void DrawInspectorContent()
	{
#if UNITY_EDITOR
		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("MOVE:", EditorStyles.boldLabel);
			EditorGUILayout.Vector2IntField("fromCoord: ", fromCoord);
			EditorGUILayout.Vector2IntField("toCoord: ", toCoord);
			
			data.DrawOpData();
		}
#endif
	}

	public GroundMoveOp(
		Unit mover, 
		Vector2Int fromCoord, 
		Vector2Int toCoord,
		float startTime,
		float duration
		)
	{
		this.fromCoord = fromCoord;
		this.toCoord = toCoord;

		startPos = Board.OffsetToWorld(fromCoord);
		endPos = Board.OffsetToWorld(toCoord);

		this._data = new OpData(mover, startTime, duration);
	}

	public void Execute(Unit unit)
	{
		unit.DecrementMove();
		unit.SetVisualPos(Vector3.zero, true);
		unit.MoveTo(toCoord);
	}

	public void Undo(Unit unit)
	{
		unit.IncrementMove();
		unit.SetVisualPos(Vector3.zero, true);
		unit.MoveTo(fromCoord);
	}

	public void Tick(Unit unit, float t)
	{
		Debug.LogWarning($"Ticking ground move: {t}");
		unit.SetVisualPos(Vector3.Lerp(startPos, endPos, t));
	}
}


//[Serializable]
//public class GroundMoveOp : UnitOp
//{
//	public Vector2Int fromCoord;
//	public Vector2Int toCoord;

//	public Vector3 startPos;
//	public Vector3 endPos;

//	public override void DrawInspectorContent()
//	{
//#if UNITY_EDITOR
//		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
//		{
//			EditorGUILayout.LabelField("MOVE:", EditorStyles.boldLabel);
//			EditorGUILayout.ObjectField(unit, typeof(Unit), true);
//			EditorGUILayout.Vector2IntField("fromCoord: ", fromCoord);
//			EditorGUILayout.Vector2IntField("toCoord: ", toCoord);
//		}
//#endif
//	}

//	public GroundMoveOp(
//		Unit mover,
//		Vector2Int fromCoord,
//		Vector2Int toCoord,
//		float duration
//		) : base(mover)
//	{
//		this.fromCoord = fromCoord;
//		this.toCoord = toCoord;
//		this.duration = duration;
//		startPos = Board.OffsetToWorld(fromCoord);
//		endPos = Board.OffsetToWorld(toCoord);
//	}

//	public override void Execute(Unit unit)
//	{
//		unit.DecrementMove();
//		unit.SetVisualPos(Vector3.zero, true);
//		unit.MoveTo(toCoord);
//	}

//	public override void Undo(Unit unit)
//	{
//		unit.IncrementMove();
//		unit.SetVisualPos(Vector3.zero, true);
//		unit.MoveTo(fromCoord);
//	}

//	public override void Tick(Unit unit, float t)
//	{
//		unit.SetVisualPos(Vector3.Lerp(startPos, endPos, t));
//	}
//}