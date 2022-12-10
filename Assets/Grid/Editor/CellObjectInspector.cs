using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellObject), true)]
public class CellObjectInspector : Editor
{
	public override void OnInspectorGUI()
	{
		CellObject cellObj = target as CellObject;

		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Bind in place"))
			cellObj.BindInPlace();
		EditorGUILayout.EndHorizontal();
	}

	private void OnEnable()
	{
		//Debug.LogWarning("ENABLED CELL OBJECT");
		//CellObject cellObj = target as CellObject;
		//cellObj.BindInPlace();
	}

	private void OnDisable()
	{
		//Debug.LogWarning("DISABLED CELL OBJECT");
		//CellObject cellObj = target as CellObject;
		//cellObj.Unbind();
	}

	private void OnSceneGUI()
	{
		var cellObject = target as CellObject;
		//var currControlID = GUIUtility.GetControlID(FocusType.Passive);

		Event e = Event.current;

		bool holdinShift = false;
		// You'll need a control id to avoid messing with other tools!
		int controlID = GUIUtility.GetControlID(FocusType.Passive);

		if (Event.current.GetTypeForControl(controlID) == EventType.KeyDown)
		{
			if (Event.current.keyCode == KeyCode.LeftShift)
			{
				//Debug.Log("shift pressed!");
				holdinShift = true;
				// Causes repaint & accepts event has been handled
				Event.current.Use();
			}
		}


		if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit hit;

			if (holdinShift)
				Debug.Log("... with SHIFT!!");

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, HexGrid.Mask))
			{
				var hitCell = hit.transform.GetComponentInParent<Cell>();

				if (hitCell != null)
				{
					Undo.RecordObject(cellObject, "Snap CelObject To Grid");
					Event.current.Use();

					if(cellObject.TryGetBoundCell(out Cell boundCell))
					{
						if (hitCell == boundCell)
						{
							//Debug.Log("hit currently bound cell!");

							var dir = hitCell.transform.position.FlatTo(hit.point);
							var closestCardinalDir = dir.ClosestCardinal();

							cellObject.SetFacing(closestCardinalDir);
							return;
						}
					}

					cellObject.MoveAndBindTo(hitCell);
				}
			}
		}
	}
}
