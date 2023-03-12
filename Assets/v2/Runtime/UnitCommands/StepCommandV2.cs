using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StepCommandV2 : UnitCommand
{
    public Vector2Int toCoord;
    public Vector2Int fromCoord;

	public Vector3 startPos;
	public Vector3 endPos;

	public StepCommandV2(Unit unit, Vector2Int fromCoord, Vector2Int toCoord, float duration)
	{
        this.unit = unit;
        this.toCoord = toCoord;
        this.fromCoord = fromCoord;
        this.duration = duration;

		startPos = Board.OffsetToWorld(fromCoord);
		endPos = Board.OffsetToWorld(toCoord);
	}

	public override void OnBeginTick()
	{
		Board.OnUnitExitedCell(unit, fromCoord);
		Board.RespondToCommandBeginTick(unit, this);
	}

	public override void OnBeginReverseTick()
	{
		Board.OnUnitExitedCell(unit, toCoord);
	}

	public override void OnCompleteTick()
	{
		Board.OnUnitEnteredCell(unit, toCoord);
		Board.RespondToCommandCompleteTick(unit, this);
	}

	public override void OnCompleteReverseTick()
	{
		Board.OnUnitEnteredCell(unit, fromCoord);
	}

	public override void Execute()
	{
		unit.DecrementMove();
		unit.SetVisualPos(Vector3.zero, true);
		unit.MoveTo(toCoord);

		//Debug.LogWarning("unit pos now: " + toCoord);
	}

	public override void Undo()
	{
		unit.IncrementMove();
		unit.SetVisualPos(Vector3.zero, true);	
		unit.MoveTo(fromCoord);
	}

	public override bool Tick(float timeScale = 1f)
	{
		base.Tick(timeScale);
		unit.SetVisualPos(Vector3.Lerp(startPos, endPos, currProgress));
		return CheckComplete(timeScale);
	}

#if UNITY_EDITOR
	public override void DrawInspectorContent()
	{
		using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
		{
			//EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("STEP", EditorStyles.boldLabel);
			EditorGUILayout.LabelField($"from: {fromCoord.ToString()}", GUILayout.Width(120f));//, typeof(Cell), true);
			EditorGUILayout.LabelField($"to: {toCoord.ToString()}", GUILayout.Width(120f));//, typeof(Cell), true);
			//EditorGUILayout.EndHorizontal();
		}
	}
#endif
}
