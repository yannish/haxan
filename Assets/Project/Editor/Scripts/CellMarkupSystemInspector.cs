using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellMarkupService))]
public class CellMarkupSystemInspector : Editor
{
	CellMarkupService system;

	private void OnEnable()
	{
		system = target as CellMarkupService;
	}

	public override void OnInspectorGUI()
	{
        DrawDefaultInspector();

		DrawButtonContent();
	}

	private void DrawButtonContent()
	{
		EditorGUILayout.Space(10);

		if(GUILayout.Button("Clear Markers"))
		{
			system?.ClearMarkers();
		}
	}

	private void OnSceneGUI()
	{
		if (!Application.isPlaying)
			return;

		CellMarkupService system = target as CellMarkupService;

		Event e = Event.current;

		if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, HexGrid.Mask))
			{
				var hitCell = hit.transform.GetComponentInParent<Cell>();

				if (hitCell != null)
				{
					Debug.LogWarning("Placing marker!");

					Undo.RecordObject(system, "Snap Marker To Grid");
					
					Event.current.Use();

					var dir = hitCell.transform.position.FlatTo(hit.point);
					var closestCardinalDir = dir.ClosestCardinal();

					system.MarkCellPush(hitCell, closestCardinalDir);
					
					//var cloestCardinalVector = closestCardinalDir.ToVector();
					//system.pushMarker.GetAndPlay(hitCell.occupantPivot.position, cloestCardinalVector);

					//if (cellObject.TryGetBoundCell(out Cell boundCell))
					//{
					//	if (hitCell == boundCell)
					//	{
					//		//Debug.Log("hit currently bound cell!");



					//		cellObject.SetFacing(closestCardinalDir);
					//		return;
					//	}
					//}

					//cellObject.MoveAndBindTo(hitCell);
				}
			}
		}
	}
}
