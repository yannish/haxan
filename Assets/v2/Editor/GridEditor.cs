using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    public static Vector2Int NumCells = new Vector2Int(4, 4);

    public override void OnInspectorGUI()
    {
        NumCells = EditorGUILayout.Vector2IntField("Number of cells to fill", NumCells);

        if (GUILayout.Button($"Clear and fill with {NumCells.x}x{NumCells.y} cells"))
        {
            Grid grid = (Grid)target;
            // Destroy all children that are cells
            CellV2[] cells = grid.GetComponentsInChildren<CellV2>();
            foreach (CellV2 cell in cells)
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
                    child.transform.localPosition = new Vector3(
                        (0.5f + x + y * 0.5f - y / 2) * CellV2.InnerRadius * 2f,
                        0f,
                        CellV2.OuterRadius + y * (CellV2.OuterRadius * 1.5f)
                    );
                }
            }
        }
    }
}