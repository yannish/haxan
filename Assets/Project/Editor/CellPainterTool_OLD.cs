using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("New Cell Painter", typeof(BoardData))]
public class CellPainterTool_OLD : EditorTool
{
	const string iconPath = "Assets/Editor/ToolIcons/";

	// Never assigned warning
#pragma warning disable CS0649
	[SerializeField]
	Texture2D m_ToolIcon;
#pragma warning restore CS0649
	GUIContent m_IconContent;
	string m_DisplayName;
	string m_Tooltip;

	private CellPainterWindow window;
	private BoardData boardData;

	private void OnEnable()
	{
        Debug.LogWarning("Enabled painter tool.");
		boardData = target as BoardData;
		if (boardData != null)
			Debug.LogWarning("... have board data!");

		this.m_ToolIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath + "PaintIcon.png");

		m_IconContent = new GUIContent()
		{
			image = m_ToolIcon,
			text = m_DisplayName,
			tooltip = m_Tooltip,
		};
	}
	
	public override GUIContent toolbarIcon => m_IconContent;

	public override void OnActivated()
	{
		Debug.LogWarning("Activated painter tool.");
		window = EditorWindow.GetWindow<CellPainterWindow>();
		//SceneView.duringSceneGui -= DrawColorSwatch;
		//SceneView.duringSceneGui += DrawColorSwatch;
	}

	public override void OnWillBeDeactivated()
	{
		Debug.LogWarning("Deactivated painter tool.");
		base.OnWillBeDeactivated();
		//SceneView.duringSceneGui -= DrawColorSwatch;
	}


	public override void OnToolGUI(EditorWindow sceneWindow)
	{
		if (boardData == null)
			return;

		if (!(sceneWindow is SceneView))
			return;

		//If we're not the active tool, exit.
		if (!ToolManager.IsActiveTool(this))
			return;

		Vector3 currMousePos = GetCurrentMousePositionInScene();
		Handles.DrawWireDisc(GetCurrentMousePositionInScene(), Vector3.up, 0.5f);

		// Override Editor HotControls to capture cursor
		if (Event.current.type == EventType.Layout)
		{
			HandleUtility.AddDefaultControl(0);
		}

		Vector2Int currCoord = Board.WorldToOffset(currMousePos);
		if(boardData.indexToCellLookup.TryGetValue(currCoord.ToIndex(), out var foundCell))
		{
			if (Event.current.type == EventType.Repaint)
			{
				var renderer = foundCell.GetComponentInChildren<Renderer>();
				var listOfRenderers = new Renderer[1];
				listOfRenderers[0] = renderer;
				if (Event.current.type == EventType.Repaint)
					Handles.DrawOutline(listOfRenderers, Color.green, 0.35f);
			}

			if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				Debug.LogWarning("right clickin in tool!");

				Undo.RecordObject(boardData, "Painted Cell");
				Event.current.Use();
				CellSwatch swatchToPaint = window.SelectedSwatch;
				if(swatchToPaint != null)
				{
					PaintCell(foundCell, swatchToPaint);
				}
			}
		}

		sceneWindow.Repaint();

		// You can access settings now
		//int foo = window.someSetting;
	}

	private void PaintCell(Cell foundCell, CellSwatch swatchToPaint)
	{
		Debug.LogWarning("PAINTING CELL!");

		var allBrushStrokes = foundCell.GetComponentsInChildren<CellBrushStroke>();
		foreach(var brushStroke in allBrushStrokes)
		{
			DestroyImmediate(brushStroke.gameObject);
		}

		var strokeToInstantiate = swatchToPaint.strokes[UnityEngine.Random.Range(0, swatchToPaint.strokes.Count)];
		if (strokeToInstantiate == null)
			return;

		var brushStrokeInstance = PrefabUtility.InstantiatePrefab(
			strokeToInstantiate.gameObject,
			foundCell.pivot
			) as GameObject;

		brushStrokeInstance.transform.rotation = Quaternion.AngleAxis(
			UnityEngine.Random.Range(0, 6),
			Vector3.up
			);

		//if (brushStrokeInstance == null)
		//	return;

		//var gameObj = (GameObject)brushStrokeInstance;
		//if (gameObj == null)
		//	return;

		//var pivot = foundCell.transform.GetChild()
	}

	Vector3 GetCurrentMousePositionInScene()
	{
		Vector3 mousePosition = Event.current.mousePosition;
		var placeObject = HandleUtility.PlaceObject(mousePosition, out var newPosition, out var normal);
		return placeObject ? newPosition : HandleUtility.GUIPointToWorldRay(mousePosition).GetPoint(10);
	}

	// Bonus shortcut to activate tool when scene is focused and 'M' pressed
	//[UnityEditor.ShortcutManagement.Shortcut("Cell Painter Tool", null, KeyCode.C)]
	static void ToolShortcut()
	{
		if (Selection.GetFiltered<BoardData>(SelectionMode.TopLevel).Length > 0)
			ToolManager.SetActiveTool<CellPainterTool_OLD>();
	}
}
