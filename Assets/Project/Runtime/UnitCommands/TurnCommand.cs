using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TurnCommand : UnitCommand
{
    public HexDirectionFT fromDir;
    public HexDirectionFT toDir;

    private Vector3 currFacing;
    private Vector3 endFacing;

    public TurnCommand(
        Unit unit,
        HexDirectionFT fromDir, 
        HexDirectionFT toDir,
        float duration
        )
	{
        this.unit = unit;
        this.fromDir = fromDir;
        this.toDir = toDir;
        this.duration = duration;

        this.currFacing = fromDir.ToVector();
        this.endFacing = toDir.ToVector();
	}

    public override void Execute() => unit.SetFacing(toDir);

    public override void Undo() => unit.SetFacing(fromDir);

    public override bool Tick_OLD(float timeScale = 1f)
	{
        base.Tick_OLD(timeScale);
        unit.SetDirectFacing(Vector3.Slerp(currFacing, endFacing, currProgress));
        return CheckComplete(timeScale);
	}

    public override bool StepsTimeForward() => false;

#if UNITY_EDITOR
    public override void DrawInspectorContent()
    {
        using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
		{
            //EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TURN", EditorStyles.boldLabel);
            EditorGUILayout.ObjectField(unit, typeof(Unit), true);
            EditorGUILayout.LabelField($"from: {fromDir}", GUILayout.Width(120f));
            EditorGUILayout.LabelField($"to: {toDir}", GUILayout.Width(120f));
            //EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
