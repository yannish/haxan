using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class TurnCommand : CellObjectCommand
{
	public HexDirection fromDir;
	public HexDirection toDir;

	private Vector3 currFacing;
	private Vector3 endFacing;


	public TurnCommand(
		CellObject cellObject, 
		HexDirection fromDir, HexDirection toDir,
		float duration
		) : base (cellObject)
	{
		this.fromDir = fromDir;
		this.toDir = toDir;
		this.duration = duration;

		this.currFacing = fromDir.ToVector();
		this.endFacing = toDir.ToVector();
	}

	public override void Execute()
	{
		cellObject.SetFacing(toDir);
	}

	public override void Undo()
	{
		cellObject.SetFacing(fromDir);
	}

	public override bool Tick(float timeScale = 1f)
	{
		//currFacing = ;
		base.Tick(timeScale);
		
		cellObject.SetDirectFacing(Vector3.Slerp(currFacing, endFacing, currProgress));

		return CheckComplete(timeScale);
	}

#if UNITY_EDITOR
	public override void DrawInspectorContent()
	{
		EditorGUILayout.EnumFlagsField("from:", fromDir);
		EditorGUILayout.EnumFlagsField("to:", toDir);
	}
#endif
}
