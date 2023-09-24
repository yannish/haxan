using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SkidCommand : UnitCommand
{
    public const string kDampSlidePath = "Prefabs/Sequences/DampSlideSequence";

    public Vector2Int toCoord;
    public Vector2Int fromCoord;

    public Vector3 startPos;
    public Vector3 endPos;

    public SkidCommand(Unit unit, Vector2Int fromCoord, Vector2Int toCoord, float duration)
	{
        this.unit = unit;
        this.toCoord = toCoord;
        this.fromCoord = fromCoord;
        this.duration = duration;

        startPos = Board.OffsetToWorld(fromCoord);
        endPos = Board.OffsetToWorld(toCoord);
    }

	public override void Execute()
	{
        unit.SetVisualPos(Vector3.zero, true);
        unit.MoveTo(toCoord);
	}

	public override void Undo()
	{
        unit.SetVisualPos(Vector3.zero, true);
        unit.MoveTo(fromCoord);
	}

	public override bool Tick_OLD(float timeScale = 1)
	{
        base.Tick_OLD(timeScale);
        unit.SetVisualPos(Vector3.Lerp(startPos, endPos, currProgress));
        return CheckComplete(timeScale);
	}

#if UNITY_EDITOR
	public override void DrawInspectorContent()
	{
		using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.LabelField("STEP", EditorStyles.boldLabel);
			EditorGUILayout.LabelField($"from: {fromCoord.ToString()}", GUILayout.Width(120f));
			EditorGUILayout.LabelField($"to: {toCoord.ToString()}", GUILayout.Width(120f));
		}
	}
#endif
}
