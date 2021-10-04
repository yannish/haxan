using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(CellPainter)), CanEditMultipleObjects]
public class CellPainterInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawScriptField();

		serializedObject.Update();
		list.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	}

	protected void DrawScriptField()
	{
		GUI.enabled = false;
		SerializedProperty prop = serializedObject.FindProperty("m_Script");
		EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
		GUI.enabled = true;
	}


	private ReorderableList list;

	private void OnEnable()
	{
		list = new ReorderableList(
			serializedObject,
			serializedObject.FindProperty("presets"),
			true, true, true, true
			);

		list.drawHeaderCallback =
			(Rect rect) =>
			{
				EditorGUI.LabelField(rect, "Cell Presets");
			};

		list.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				rect.y += 2;

				EditorGUI.PropertyField(
					new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
					element,
					GUIContent.none
					);
			};

	}

	private void OnSceneGUI()
	{
		var cellPainter = target as CellPainter;

		Event e = Event.current;

		if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
		{
			//Debug.Log("CLICK IN CELLPAINTER");

			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("gridmesh")))
			{
				//Debug.Log("CLICK IN CELLPAINTER HIT SOMETHING");

				if (hit.transform.GetComponentInParent<Cell>() != null)
				{
					Undo.RecordObject(cellPainter, "Painted Tile");
					Event.current.Use();
					var paintedCell = hit.transform.GetComponentInParent<Cell>();

					if (!cellPainter.presets.IsNullOrEmpty())
						CellPainter.PaintCell(paintedCell, cellPainter.presets[0]);
				}
			}
		}
	}
}
