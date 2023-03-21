using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[CustomEditor(typeof(GridV2))]
public class GridV2Editor : Editor
{
    public static Vector2Int NumCells = new Vector2Int(4, 4);

    public override void OnInspectorGUI()
    {
        serializedObject.DrawScriptField();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cells"));

        NumCells = EditorGUILayout.Vector2IntField("Number of cells to fill", NumCells);

        if (GUILayout.Button($"Clear and fill with {NumCells.x}x{NumCells.y} cells"))
        {
            GridV2 grid = (GridV2)target;
            
            // Destroy all children that are cells
            Cell[] cells = grid.GetComponentsInChildren<Cell>();
            foreach (Cell cell in cells)
            {
                DestroyImmediate(cell.gameObject);
            }

            grid.cells.Clear();

            // Create new cells
            var prefab = Resources.Load("Prefabs/SimpleCell");
            for (int x = 0; x < NumCells.x; x++)
            {
                for (int y = 0; y < NumCells.y; y++)
                {
                    grid.AddCell(new Vector2Int(x, y), prefab);

                    //Cell newCell = ((GameObject)Instantiate(prefab, grid.transform)).GetComponent<Cell>();
                    //newCell.name = $"Cell";
                    //newCell.transform.localPosition = Board.OffsetToWorld(new Vector2Int(x, y));
                    //grid.cells.Add(newCell);
                }
            }

#if UNITY_EDITOR
            BoardManager.RefreshBoardData();
            //EditorSceneManager.MarkSceneDirty(grid.gameObject.scene);
 #endif

            //Scene scene = grid.gameObject.scene;
            //BoardData boardData = FindObjectOfType<BoardData>();
            //if (boardData != null)
            //    boardData.Refresh();
        }
    }
}

public static class GridActions
{
    public static void AddCell(this GridV2 grid, Vector2Int coord, Object cellPrefab)
	{
        Cell newCell = ((GameObject)GameObject.Instantiate(cellPrefab, grid.transform)).GetComponent<Cell>();
        newCell.name = $"Cell";
        newCell.transform.localPosition = Board.OffsetToWorld(coord);
        grid.cells.Add(newCell);
    }
}