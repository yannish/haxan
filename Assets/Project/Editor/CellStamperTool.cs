using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Cell Stamp Tool", typeof(GridV2))]
class CellStamperTool : BaseEditorTool
{
    internal override EventModifiers WhitelistModifiers => EventModifiers.Shift;

    public CellStamperTool()
	{
        displayName = "Stamp Tool";
        tooltip = "Add & remove cells in this grid.";

        Debug.LogWarning("Grabbin a quick tool");
    }

    internal override string iconName => "StampIcon.png";

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

	Vector3 currMousePos;
    Vector2Int currCoord;
    Cell hoveredCell;
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
                Color drawColor = IsEraser() ? Color.red : Color.grey;
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
                Color drawColor = Color.green;
                Handles.DrawOutline(listOfRenderers, drawColor, 0.2f);
            }
        }
    }

    protected virtual bool IsEraser() => Event.current.modifiers == EventModifiers.Shift;


    bool needsRefresh;
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
            if (hoveredCell == null)
			{
                StampCell(currCoord);
                //grid.AddCell(currCoord, cellPrefab);
                //needsRefresh = true;
                //Cell newCell 
			}
		}
		else
		{
            if (hoveredCell != null)
			{
                DeleteCell(hoveredCell);
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
            Debug.LogWarning("DRAG IN STAMP TOOL");
            if(hoveredCell == null)
                StampCell(currCoord);
        }
        else
        {
            Debug.LogWarning("ERASER DRAG IN STEMP TOOL");
            if (hoveredCell != null)
                DeleteCell(hoveredCell);
        }
    }

    void StampCell(Vector2Int coord)
	{
        Debug.LogWarning("ADDING CELL");
        grid.AddCell(coord, cellPrefab);
        needsRefresh = true;
	}

    void DeleteCell(Cell cell)
	{
        Debug.LogWarning("ERASING CELL");
        DestroyImmediate(hoveredCell.gameObject);
        needsRefresh = true;
    }

    [UnityEditor.ShortcutManagement.Shortcut("Cell Stamper Tool", null, KeyCode.V)]
    static void ToolShortcut()
    {
        if (Selection.GetFiltered<GridV2>(SelectionMode.TopLevel).Length > 0)
            ToolManager.SetActiveTool<CellStamperTool>();
    }
}
