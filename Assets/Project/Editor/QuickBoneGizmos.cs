using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class QuickBoneGizmos
{
	[DrawGizmo(
		GizmoType.InSelectionHierarchy |
		GizmoType.Selected |
		GizmoType.Pickable
		)]
	public static void OnDrawSceneGizmo(QuickBone bone, GizmoType gizmoType)
	{
		Color drawColor = ColorPicker.Swatches.probe;

		using (new ColorContext(drawColor))
		{
			Gizmos.DrawSphere(
				bone.transform.position,
				bone.transform.lossyScale.x * 0.3f
				);
		}
	}
}
