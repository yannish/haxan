using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TurnOp : UnitOp
{
	public HexDirectionFT fromDir;
	public HexDirectionFT toDir;

	private Vector3 currFacing;
	private Vector3 endFacing;


	public override void DrawInspectorContent()
	{
#if UNITY_EDITOR
		using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("TURN", EditorStyles.boldLabel);
			EditorGUILayout.ObjectField(unit, typeof(Unit), true);
			EditorGUILayout.LabelField($"from: {fromDir}", GUILayout.Width(120f));
			EditorGUILayout.LabelField($"to: {toDir}", GUILayout.Width(120f));
		}
#endif
	}

	public TurnOp(
		Unit unit, 
		HexDirectionFT fromDir, 
		HexDirectionFT toDir, 
		float duration
		) : base(unit)
	{
		this.fromDir = fromDir;
		this.toDir = toDir;
		this.duration = duration;
		this.currFacing = fromDir.ToVector();
		this.endFacing = fromDir.ToVector();
	}

	public override void Execute(Unit unit) => unit.SetFacing(toDir);

	public override void Undo(Unit unit) => unit.SetFacing(fromDir);

	public override void Tick(Unit unit, float t)
	{
		unit.SetDirectFacing(Vector3.Slerp(currFacing, endFacing, t));
	}
}
