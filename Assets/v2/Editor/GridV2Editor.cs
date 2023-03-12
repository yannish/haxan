using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridV2))]
public class GridV2Editor : Editor
{
    public static Vector2Int NumCells = new Vector2Int(4, 4);

    public override void OnInspectorGUI()
    {
        serializedObject.DrawScriptField();

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

            // Create new cells
            var prefab = Resources.Load("Prefabs/SimpleCell");
            for (int x = 0; x < NumCells.x; x++)
            {
                for (int y = 0; y < NumCells.y; y++)
                {
                    GameObject child = (GameObject)Instantiate(prefab, grid.transform);
                    child.name = $"Cell";
                    child.transform.localPosition = Board.OffsetToWorld(new Vector2Int(x, y));
                }
            }
        }
    }
}