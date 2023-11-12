using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class BashOp : UnitOpClass
{
	public Unit hitter;
	public Unit target;

	Vector2Int fromPos;
	Vector2Int toPos;

	public BashOp(Unit unit, Unit target, Vector2Int fromPos, Vector2Int toPos) : base(unit)
	{
		this.hitter = unit;
		this.target = target;
		this.fromPos = fromPos;
		this.toPos = toPos;
	}

	public override void Execute(Unit unit)
	{

	}

	public override void Undo(Unit unit)
	{

	}

	public override void Tick(Unit unit, float t)
	{

	}

	public override void DrawInspectorContent()
	{
#if UNITY_EDITOR
		using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("BASH", EditorStyles.boldLabel);
			EditorGUILayout.ObjectField($"target:", target, typeof(Unit), true);
			//EditorGUILayout.ObjectField($"target:", target, typeof(Unit), true, GUILayout.Width(120f));
			EditorGUILayout.ObjectField($"hitter:", hitter, typeof(Unit), true);
			//EditorGUILayout.ObjectField($"hitter:", hitter, typeof(Unit), true, GUILayout.Width(120f));
		}
#endif
	}
}