using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TurnOp : IUnitOperable
{
	public HexDirectionFT fromDir;
	public HexDirectionFT toDir;

	private Vector3 currFacing;
	private Vector3 endFacing;

	public OpPlaybackData _data;
	public OpPlaybackData data => _data;

	public void DrawInspectorContent()
	{
#if UNITY_EDITOR
		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("TURN", EditorStyles.boldLabel, GUILayout.MaxWidth(100f));
			EditorGUILayout.ObjectField(_data.unitIndex.ToUnit(), typeof(Unit), true);
			EditorGUILayout.LabelField($"from: {fromDir}", GUILayout.Width(120f));
			EditorGUILayout.LabelField($"to: {toDir}", GUILayout.Width(120f));
			//
			_data.DrawOpData();
		}
#endif
	}

	public TurnOp(
		Unit unit, 
		HexDirectionFT fromDir, 
		HexDirectionFT toDir, 
		float startTime,
		float duration
		)
	{
		this.fromDir = fromDir;
		this.toDir = toDir;
		this.currFacing = fromDir.ToVector();
		this.endFacing = toDir.ToVector();

		this._data = new OpPlaybackData(unit, startTime, duration);
	}

	public void Execute(Unit unit) => unit.SetFacing(toDir);

	public void Undo(Unit unit) => unit.SetFacing(fromDir);

	public void Tick(Unit unit, float t)
	{
		Debug.LogWarning($"Ticking turn: {t}");
		unit.SetDirectFacing(Vector3.Slerp(currFacing, endFacing, t));
	}
}


//public class TurnOp : UnitOp
//{
//	public HexDirectionFT fromDir;
//	public HexDirectionFT toDir;

//	private Vector3 currFacing;
//	private Vector3 endFacing;


//	public override void DrawInspectorContent()
//	{
//#if UNITY_EDITOR
//		using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
//		{
//			EditorGUILayout.LabelField("TURN", EditorStyles.boldLabel);
//			EditorGUILayout.ObjectField(unit, typeof(Unit), true);
//			EditorGUILayout.LabelField($"from: {fromDir}", GUILayout.Width(120f));
//			EditorGUILayout.LabelField($"to: {toDir}", GUILayout.Width(120f));
//		}
//#endif
//	}

//	public TurnOp(
//		Unit unit,
//		HexDirectionFT fromDir,
//		HexDirectionFT toDir,
//		float duration
//		) : base(unit)
//	{
//		this.fromDir = fromDir;
//		this.toDir = toDir;
//		this.duration = duration;
//		this.currFacing = fromDir.ToVector();
//		this.endFacing = fromDir.ToVector();
//	}

//	public override void Execute(Unit unit) => unit.SetFacing(toDir);

//	public override void Undo(Unit unit) => unit.SetFacing(fromDir);

//	public override void Tick(Unit unit, float t)
//	{
//		unit.SetDirectFacing(Vector3.Slerp(currFacing, endFacing, t));
//	}
//}
