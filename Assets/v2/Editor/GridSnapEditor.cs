using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSnap), true)]
public class GridSnapEditor : Editor
{
	private void OnSceneGUI()
	{
		GridSnap gridSnap = target as GridSnap;
		if (gridSnap == null || gridSnap.Pivot == null)
			return;

		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.RightArrow)
		{
			var newDir = gridSnap.Facing.Next();
			gridSnap.Pivot.SetFacing(newDir);
			gridSnap.Facing = newDir;
			Event.current.Use();
		}

		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftArrow)
		{
			var newDir = gridSnap.Facing.Previous();
			gridSnap.Pivot.SetFacing(newDir);
			gridSnap.Facing = newDir;
			Event.current.Use();
		}
	}
}