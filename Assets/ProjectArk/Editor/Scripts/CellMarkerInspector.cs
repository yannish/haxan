using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellMarker_OLD))]
public class CellMarkerInspector : Editor
{
	private void OnSceneGUI()
	{
		var cellMarker = target as CellMarker_OLD;
		//var currControlID = GUIUtility.GetControlID(FocusType.Passive);

		Event e = Event.current;

		//bool holdinShift = false;

		// You'll need a control id to avoid messing with other tools!
		int controlID = GUIUtility.GetControlID(FocusType.Passive);

		//if (Event.current.GetTypeForControl(controlID) == EventType.KeyDown)
		//{
		//	if (Event.current.keyCode == KeyCode.LeftShift)
		//	{
		//		holdinShift = true;
		//		// Causes repaint & accepts event has been handled
		//		Event.current.Use();
		//	}
		//}


		if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit hit;

			//if (holdinShift)
			//	Debug.Log("... with SHIFT!!");

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, HexGrid.Mask))
			{
				var hitCell = hit.transform.GetComponentInParent<Cell_OLD>();
				if (hitCell != null)
				{
					Undo.RecordObject(cellMarker, "Snap Cellmarker To Grid");
					Event.current.Use();

					cellMarker.transform.position = hitCell.transform.position;

					//if (cellMarker.TryGetBoundCell(out Cell boundCell))
					//{
					//	if (hitCell == boundCell)
					//	{
					//		//Debug.Log("hit currently bound cell!");

					//		var dir = hitCell.transform.position.FlatTo(hit.point);
					//		var closestCardinalDir = dir.ClosestCardinal();

					//		cellMarker.SetFacing(closestCardinalDir);
					//		return;
					//	}
					//}

					//cellMarker.MoveAndBindTo(hitCell);
				}
			}
		}
	}
}
