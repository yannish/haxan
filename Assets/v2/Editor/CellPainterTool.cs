using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Cell Paint Tool", typeof(GridV2))]
class CellPainterTool : BaseEditorTool
{
	internal override EventModifiers WhitelistModifiers => EventModifiers.Shift; 
	internal override string iconName => "PaintIcon.png";

	public CellPainterTool()
	{
		displayName = "Paint Tool";
		tooltip = "Apply & Clear surface flags on cells in this grid.";
	}

	private CellPainterWindow window;
	private BoardData boardData;
	private GridV2 grid;

	private GameObject dummyCellInstance;
	private Object cellPrefab;
	private Renderer cellRenderer;
	Renderer[] listOfRenderers = new Renderer[1];
	public override void OnActivated()
	{
		Debug.LogWarning("Activated stamper tool.");
		window = EditorWindow.GetWindow<CellPainterWindow>();
		boardData = FindObjectOfType<BoardData>();
		grid = target as GridV2;

		var dummyCellPrefab = Resources.Load("Prefabs/DummyToolCell") as GameObject;
		dummyCellInstance = (GameObject)Instantiate(dummyCellPrefab);
		cellRenderer = dummyCellInstance.GetComponentInChildren<Renderer>();

		cellPrefab = Resources.Load("Prefabs/SimpleCell") as GameObject;
		//cellRenderer.enabled = false;
		//cellInstance.hideFlags = 
	}

	public override void OnWillBeDeactivated()
	{
		DestroyImmediate(dummyCellInstance);
	}

	protected virtual bool IsEraser() => Event.current.modifiers == EventModifiers.Shift;

	Vector3 currMousePos;
	Vector2Int currCoord;
	Cell hoveredCell;
	private bool needsRefresh;

	public override void DrawHandles()
	{
		currMousePos = GetCurrentMousePositionInScene();
		Handles.DrawWireDisc(currMousePos, Vector3.up, 1f, 2f);

		currCoord = Board.WorldToOffset(currMousePos);

		hoveredCell = null;
		needsRefresh = false;

		if (boardData.indexToCellLookup.TryGetValue(currCoord.ToIndex(), out var foundCell))
		{
			hoveredCell = foundCell;

			if (Event.current.type == EventType.Repaint)
			{
				var foundRenderer = foundCell.GetComponentInChildren<Renderer>();
				//var listOfRenderers = new Renderer[1];
				listOfRenderers[0] = foundRenderer;
				Color drawColor = IsEraser() ? Color.red : Color.green;
				Handles.DrawOutline(listOfRenderers, drawColor, 0.2f);
			}
		}
		else
		{
			if (Event.current.type == EventType.Repaint)
			{
				dummyCellInstance.transform.position = Board.OffsetToWorld(currCoord);
				//var renderer = cellInstance.GetComponentInChildren<Renderer>();
				var listOfRenderers = new Renderer[1];
				listOfRenderers[0] = cellRenderer;
				Color drawColor = Color.grey;
				Handles.DrawOutline(listOfRenderers, drawColor, 0.2f);
			}
		}
	}

	public override void LateTick()
	{
		if (!needsRefresh)
			return;

		boardData.Refresh();
		needsRefresh = false;
	}

	public override void OnMouseDown()
	{
		if (!IsEraser())
		{
			Debug.LogWarning("BRUSH DOWN IN PAINT TOOL");

			if (
				hoveredCell != null
				&& window != null 
				&& window.SelectedSwatch != null
				)
			{
				PaintCell(hoveredCell, window.SelectedSwatch);
			}
		}
		else
		{
			if (hoveredCell != null)
			{
				ClearCell(hoveredCell);
				//Debug.LogWarning("ERASING CELL");
				//DestroyImmediate(hoveredCell.gameObject);
				//needsRefresh = true;
			}
		}
	}

	public override void OnMouseUp()
	{
		Debug.LogWarning("... mouse up in quick tool.");
		if (needsRefresh)
		{
			boardData.Refresh();
		}
	}

	public override void OnMouseDrag()
	{
		if (!IsEraser())
		{
			Debug.LogWarning("DRAG IN BRUSH TOOL");
		}
		else
		{
			Debug.LogWarning("ERASER DRAG IN BRUSH TOOL");
		}
	}

	private void ClearCell(Cell foundCell)
	{
		var allBrushStrokes = foundCell.GetComponentsInChildren<CellBrushStroke>();
		foreach (var brushStroke in allBrushStrokes)
			DestroyImmediate(brushStroke.gameObject);

		foundCell.surfaceFlags = 0;
		needsRefresh = true;
	}

	private void PaintCell(Cell foundCell, CellSwatch swatchToPaint)
	{
		Debug.LogWarning("PAINTING CELL!");

		var allBrushStrokes = foundCell.GetComponentsInChildren<CellBrushStroke>();
		foreach (var brushStroke in allBrushStrokes)
			DestroyImmediate(brushStroke.gameObject);

		foundCell.surfaceFlags = swatchToPaint.cellState;

		//var strokeToInstantiate = swatchToPaint.strokes[UnityEngine.Random.Range(0, swatchToPaint.strokes.Count)];
		//if (strokeToInstantiate == null)
		//	return;

		var brushStrokeInstance = PrefabUtility.InstantiatePrefab(
			swatchToPaint.strokes[UnityEngine.Random.Range(0, swatchToPaint.strokes.Count)].gameObject,
			foundCell.pivot
			) as GameObject;

		brushStrokeInstance.transform.localPosition = Vector3.zero;
		brushStrokeInstance.transform.rotation = Quaternion.identity;
		brushStrokeInstance.transform.rotation *= Quaternion.AngleAxis(Random.Range(0, 6) * 60f, Vector3.up);

		needsRefresh = true;

		//if (brushStrokeInstance == null)
		//	return;

		//var gameObj = (GameObject)brushStrokeInstance;
		//if (gameObj == null)
		//	return;

		//var pivot = foundCell.transform.GetChild()
	}

	[UnityEditor.ShortcutManagement.Shortcut("Cell Painter Tool", null, KeyCode.C)]
	static void ToolShortcut()
	{
		if (Selection.GetFiltered<GridV2>(SelectionMode.TopLevel).Length > 0)
			ToolManager.SetActiveTool<CellPainterTool>();
	}
}
