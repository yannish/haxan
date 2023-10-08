using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

// Custom Editor Tool for MagicComponent component
[EditorTool("Custom Tool", typeof(MagicComponent))]
public class MagicTool : EditorTool
{
    // Reference to settings window
    private MagicToolSettingsWindow window;

    // Never assigned warning
#pragma warning disable CS0649
    [SerializeField]
    Texture2D m_ToolIcon;
#pragma warning restore CS0649
    GUIContent m_IconContent;
    string m_DisplayName;
    string m_Tooltip;

    private void OnEnable()
	{

        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = m_DisplayName,
            tooltip = m_Tooltip,
        };
    }

	public override void OnActivated()
    {
        // MagicToolSettingsWindow is required to work, so open it when tool gets activated
        // When window is docked as tab, then it will be focused instead
        window = EditorWindow.GetWindow<MagicToolSettingsWindow>();
    }

    public override GUIContent toolbarIcon => m_IconContent;

    public override void OnToolGUI(EditorWindow sceneWindow)
    {
        //// Window must be SceneView
        //if (!(sceneWindow is SceneView sceneView))
        //    return;

        //If we're not in the scene view, exit.
        if (!(sceneWindow is SceneView))
            return;

        //If we're not the active tool, exit.
        if (!ToolManager.IsActiveTool(this))
            return;

        // You can access settings now
        int foo = window.someSetting;


		// Do your stuff [...]
		//Debug.LogWarning("doin tool stuff now");

		Handles.DrawWireDisc(GetCurrentMousePositionInScene(), Vector3.up, 0.5f);

        sceneWindow.Repaint();
    }

    Vector3 GetCurrentMousePositionInScene()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        var placeObject = HandleUtility.PlaceObject(mousePosition, out var newPosition, out var normal);
        return placeObject ? newPosition : HandleUtility.GUIPointToWorldRay(mousePosition).GetPoint(10);
    }

    // [...]

    // Bonus shortcut to activate tool when scene is focused and 'M' pressed
    [UnityEditor.ShortcutManagement.Shortcut("Magic Tool", null, KeyCode.M)]
    static void ToolShortcut()
    {
        if (Selection.GetFiltered<MagicComponent>(SelectionMode.TopLevel).Length > 0)
        {
            ToolManager.SetActiveTool<MagicTool>();
        }
    }
}

// Tool Settings Window
public class MagicToolSettingsWindow : EditorWindow
{
    public int someSetting = 3;

    [MenuItem("Tools/Magic Tool Window")]
    public static void OpenWindow()
    {
        MagicToolSettingsWindow window = GetWindow<MagicToolSettingsWindow>();
    }

    void OnDisable()
    {
        // Check if EditorTool is currently active and disable it when window is closed
        // MagicTool requires this window to be open as long as it's active
        if (ToolManager.activeToolType == typeof(MagicTool))
        {
            // Try to activate previously used tool
            ToolManager.RestorePreviousPersistentTool();
        }
    }

    // [...]
}

