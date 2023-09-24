using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


public interface IOperable
{
	public void Execute();

	public void Undo();

	public void DrawInspectorContext();
}

public struct UnitOp
{ 

}

[Serializable]
public struct UnitBash: IOperable
{
	public Unit target;
	public Unit hitter;

#if UNITY_EDITOR
	public void DrawInspectorContext()
	{
		using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("BASH", EditorStyles.boldLabel);
			EditorGUILayout.ObjectField($"target:", target, typeof(Unit), true);
			//EditorGUILayout.ObjectField($"target:", target, typeof(Unit), true, GUILayout.Width(120f));
			EditorGUILayout.ObjectField($"hitter:", hitter, typeof(Unit), true);
			//EditorGUILayout.ObjectField($"hitter:", hitter, typeof(Unit), true, GUILayout.Width(120f));
		}
	}
#endif

	public void Execute()
	{

	}

	public void Undo()
	{

	}
}

[Serializable]
public struct UnitMove : IOperable
{
	public Vector2Int startPos;
	public Vector2Int endPos;

	public UnitMove(Vector2Int startPos, Vector2Int endPos)
	{
		this.startPos = startPos;
		this.endPos = endPos;
	}

#if UNITY_EDITOR
	public void DrawInspectorContext()
	{
		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("MOVE:", EditorStyles.boldLabel);
			EditorGUILayout.Vector2IntField("startPos: ", startPos);
			//EditorGUILayout.Vector2IntField("startPos: ", startPos, GUILayout.Width(20f));
			EditorGUILayout.Vector2IntField("endPos: ", endPos);
			//EditorGUILayout.Vector2IntField("endPos: ", endPos, GUILayout.Width(20f));
		}
	}
#endif

	public void Execute()
	{
		
	}

	public void Undo()
	{
		
	}
}