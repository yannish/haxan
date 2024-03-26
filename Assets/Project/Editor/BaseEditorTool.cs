using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

abstract class BaseEditorTool : EditorTool
{
    const string iconPath = "Assets/Project/Editor/Tools/ToolIcons/";

    // Never assigned warning
#pragma warning disable CS0649
    [SerializeField]
    Texture2D m_ToolIcon;
#pragma warning restore CS0649

    //const string kBrushKey = "Triband.GolfTerrain.Brush";
    //const string kSelectedLayerKey = "Triband.GolfTerrain.SelectedLayerIndex";

    string m_DisplayName;
    string m_Tooltip;
    
    GUIContent m_IconContent;
    Vector3 m_CursorPosition;

    /// <summary>
    /// Override to enable processing when holding specific modifiers.
    /// </summary>
    internal virtual EventModifiers WhitelistModifiers => EventModifiers.None;

    internal virtual string iconName { get; }

    protected virtual void FetchIcon()
	{
        this.m_ToolIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath + iconName);
    }

    void OnEnable()
    {
        FetchIcon();

        // Update Icon
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = m_DisplayName,
            tooltip = m_Tooltip,
        };

        // Callbacks
        Undo.undoRedoPerformed += OnUndo;
    }

    void OnDestroy()
    {
        // Callbacks
        Undo.undoRedoPerformed -= OnUndo;
    }

    /// <summary>
    /// Display name used in EditorTool dropdown
    /// </summary>
    public string displayName
    {
        get => m_DisplayName;
        set => m_DisplayName = value;
    }

    /// <summary>
    /// Tooltip used when hoving in EditorTool dropdown
    /// </summary>
    public string tooltip
    {
        get => m_Tooltip;
        set => m_Tooltip = value;
    }

    /// <summary>
    /// Icon for the Toolbar Button.
    /// </summary>
    public override GUIContent toolbarIcon => m_IconContent;

    /// <summary>
    /// Current selected Golf Terrain
    /// </summary>
    //public GolfTerrain golfTerrain => target as GolfTerrain;

    /// <summary>
    /// Current selected Terrain Layer
    /// </summary>
    //public TerrainLayer currentLayer => m_CurrentLayer;

    /// <summary>
    /// Current selected Brush
    /// </summary>
    //public Brush currentBrush => m_CurrentBrush;

    /// <summary>
    /// Current Cursor position in World Space
    /// </summary>
    public Vector3 cursorPosition => m_CursorPosition;

    /// <summary>
    /// EditorTool override for OnToolGUI.
    /// </summary>
    /// <param name="window">The active EditorWindow.</param>
    public override void OnToolGUI(EditorWindow window)
    {
        //if (golfTerrain == null) 
        //    return;

        // Get Brush from EditorSettings
        //EditorUserSettingsUtil.TryGetUserSetting<Brush>(kBrushKey, out var brush);
        //m_CurrentBrush = brush;

        // Get active Layer using index from EditorSettings
        // Need to catch empty Layer lists here
        //var layerIndex = EditorPrefsUtil.Load<int>(kSelectedLayerKey);
        //if (layerIndex == -1 || (golfTerrain != null && layerIndex >= golfTerrain.terrainData.layers.Count))
        //    return;

        //m_CurrentLayer = golfTerrain.terrainData.layers[layerIndex];

        // Calculate the intersection point between the cursor vector and a Y-up Plane at the origin
        // Use the Layer height so the brush gizmo is alligned correctly in Y
        //var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        //var height = golfTerrain.transform.position.y;
        //var plane = new Plane(Vector3.up, new Vector3(0, height, 0));
        //m_CursorPosition = ray.GetPlaneIntersection(plane);

        //Vector3 currMousePos = GetCurrentMousePositionInScene();
        //Handles.DrawWireDisc(GetCurrentMousePositionInScene(), Vector3.up, 0.5f);

        // Override Editor HotControls to capture cursor
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
        }

        // Draw the Brush gizmo
        bool isEraser = Event.current.modifiers == EventModifiers.Shift;
        var handleColor = isEraser ? Color.black : Color.white;
        using (new Handles.DrawingScope(handleColor))
        {
            DrawHandles();
        }

        // Input
        Event evt = Event.current;

        // Only handle mouse input if either:
        // A. No modifiers are pressed
        // B. Specific modifiers are pressed. (Override WhitelistModifiers per tool type)
        if (evt.modifiers == EventModifiers.None || (evt.modifiers & WhitelistModifiers) != 0)
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    OnMouseDown();
                    break;
                case EventType.MouseUp:
                    OnMouseUp();
                    break;
                case EventType.MouseDrag:
                    OnMouseDrag();
                    break;
                case EventType.MouseLeaveWindow:
                    OnMouseLeaveWindow();
                    break;
            }
        }

        LateTick();

        // Force Repaint
        window.Repaint();
    }

    protected Vector3 GetCurrentMousePositionInScene()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        var placeObject = HandleUtility.PlaceObject(mousePosition, out var newPosition, out var normal);
        return placeObject ? newPosition : HandleUtility.GUIPointToWorldRay(mousePosition).GetPoint(10);
    }

    /// <summary>
    /// Draw Handles for the EditorTool within a Handles.DrawingScope.
    /// </summary>
    public abstract void DrawHandles();

    public virtual void LateTick() { }

    /// <summary>
    /// Called when MouseDown event is captured.
    /// </summary>
    public virtual void OnMouseDown() { }

    /// <summary>
    /// Called when MouseUp event is captured.
    /// </summary>
    public virtual void OnMouseUp() { }

    /// <summary>
    /// Called when MouseDrag event is captured.
    /// </summary>
    public virtual void OnMouseDrag() { }

    /// <summary>
    /// Called when MouseLeaveWindow event is captured.
    /// </summary>
    public virtual void OnMouseLeaveWindow() { }

    /// <summary>
    /// Called when Undo events are performed.
    /// </summary>
    public virtual void OnUndo() { }
}