using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

//[InitializeOnLoad]
public class CellPainterWindow : EditorWindow
{
    [MenuItem("Tools/CellPainter")]
    public static void ShowWindow()
	{
        CellPainterWindow window = (CellPainterWindow)EditorWindow.GetWindow(typeof(CellPainterWindow));
        window.Show();
    }

	void OnDisable()
	{
		// Check if EditorTool is currently active and disable it when window is closed
		// CellPainterTool requires this window to be open as long as it's active
		if (ToolManager.activeToolType == typeof(CellPainterTool_OLD))
		{
			// Try to activate previously used tool
			ToolManager.RestorePreviousPersistentTool();
		}
	}

	//static CellPainterWindow()
	//{
	//	SceneView.duringSceneGui -= OnDuringSceneGui;
	//	SceneView.duringSceneGui += OnDuringSceneGui;
	//}

	//private static void OnDuringSceneGui(SceneView obj)
	//{
	//	if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
	//	{
	//		if (boardData == null)
	//		{
	//			Debug.LogWarning("... but there's no board data");
	//		}

	//		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

	//		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
	//		if (groundPlane.Raycast(ray, out float dist))
	//		{
	//			Vector3 worldPos = ray.GetPoint(dist);
	//			Vector2Int coord = Board.WorldToOffset(worldPos);

	//			if (boardData.indexToCellLookup.TryGetValue(coord.ToIndex(), out var foundCell))
	//			{
	//				Debug.LogWarning($"found cell at {coord} with flags: {foundCell.surfaceFlags}");
	//			}
	//			else
	//			{
	//				Debug.LogWarning($"... no cell at {coord}");
	//			}
	//		}
	//	}
	//}

	CellSwatchCollection masterSwatchCollection;
	BoardData boardData;
	[ReadOnly] public int selectedSwatch = -1;
	private void OnEnable()
	{
		masterSwatchCollection = Resources.Load("Prefabs/Cells/Swatches/CellSwatches") as CellSwatchCollection;
		if (masterSwatchCollection != null)
			Debug.LogWarning("got the swatches!");
		
		boardData = FindObjectOfType<BoardData>();
		if(boardData != null)
			Debug.LogWarning("got the board data!");
	}

	public CellSwatch SelectedSwatch
	{
		get
		{
			//Debug.

			if (selectedSwatch < 0 || selectedSwatch >= masterSwatchCollection.swatches.Count)
				return null;

			return masterSwatchCollection.swatches[selectedSwatch];
		}
	}

	private void OnFocus()
	{
		//if (Selection.GetFiltered<BoardData>(SelectionMode.TopLevel).Length > 0)
		//{
		//	ToolManager.SetActiveTool<CellPainterTool>();
		//}
		//ToolManager.SetActiveTool<CellPainterTool>();
		//SceneView.duringSceneGui -= OnSceneGUI;
		//SceneView.duringSceneGui += OnSceneGUI;
	}

	//private void OnLostFocus()
	//{
	//	SceneView.duringSceneGui -= OnSceneGUI;
	//}

	//private void OnSceneGUI(SceneView obj)
	//{
	//	//Debug.LogWarning("ON SCENE GUI FOR PAINTER!");

	//	if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
	//	{
	//		if (boardData == null)
	//		{
	//			Debug.LogWarning("... but there's no board data");
	//		}

	//		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

	//		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
	//		if(groundPlane.Raycast(ray, out float dist))
	//		{
	//			Vector3 worldPos = ray.GetPoint(dist);
	//			Vector2Int coord = Board.WorldToOffset(worldPos);

	//			if(boardData.indexToCellLookup.TryGetValue(coord.ToIndex(), out var foundCell))
	//			{
	//				Debug.LogWarning($"found cell at {coord} with flags: {foundCell.surfaceFlags}");
	//			}
	//			else
	//			{
	//				Debug.LogWarning($"... no cell at {coord}");
	//			}
	//		}
	//	}
	//}

	private void OnGUI()
	{
		if(masterSwatchCollection == null)
		{
			GUILayout.Label("... no cell swatches found.", EditorStyles.boldLabel);
			return;
		}

		EditorGUILayout.ObjectField("SELECTED SWATCH:", masterSwatchCollection.selectedSwatch, typeof(CellSwatch), false);
		EditorGUILayout.IntField("... ", selectedSwatch);

		string[] swatchNames = new string[masterSwatchCollection.swatches.Count];
		for (int i = 0; i < masterSwatchCollection.swatches.Count; i++)
		{
			swatchNames[i] = masterSwatchCollection.swatches[i].name.ToUpper();
		}

		EditorGUI.BeginChangeCheck();
		selectedSwatch = GUILayout.SelectionGrid(selectedSwatch, swatchNames, 3);//, EditorStyles.helpBox);
		if (EditorGUI.EndChangeCheck())
		{
			if (selectedSwatch >= 0)
				masterSwatchCollection.selectedSwatch = masterSwatchCollection.swatches[selectedSwatch];
			AssetDatabase.SaveAssetIfDirty(masterSwatchCollection);
		}
		//selectedSwatch = GUILayout.Toolbar(selectedSwatch, swatchNames);//, EditorStyles.helpBox);


		//using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
		//{
		//	foreach(var swatch in masterSwatchCollection.swatches)
		//	{
		//		//GUILayout.Button(
		//		//	swatch.name.ToUpper(), 
		//		//	//EditorStyles.boldLabel, 
		//		//	GUILayout.Height(50)
		//		//	);

		//		if (GUILayout.Button(
		//			swatch.name.ToUpper(), 
		//			//EditorStyles.boldLabel, 
		//			GUILayout.Height(50))
		//			)
		//		{
		//			masterSwatchCollection.selectedSwatch = swatch;
		//		}
		//	}
		//}
	}
}
