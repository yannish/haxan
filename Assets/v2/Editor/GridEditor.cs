using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    SerializedProperty numTiles;

    void OnEnable()
    {
        numTiles = serializedObject.FindProperty("NumTiles");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(numTiles);

        if (GUILayout.Button("Clear and fill with basic cells"))
        {
            Grid grid = (Grid)target;
            // Destroy all children that are cells
            CellV2[] cells = grid.GetComponentsInChildren<CellV2>();
            foreach (CellV2 cell in cells)
            {
                DestroyImmediate(cell.gameObject);
            }
            // Create new cells
            Vector2Int num = grid.NumTiles;
            var prefab = Resources.Load("Prefabs/SimpleCell");
            for (int x = 0; x < num.x; x++)
            {
                for (int y = 0; y < num.y; y++)
                {
                    GameObject child = (GameObject)Instantiate(prefab, grid.transform);
                    child.name = $"Cell_{x}_{y}";
                    child.transform.localPosition = new Vector3(
                        (0.5f + x + y * 0.5f - y / 2) * CellV2.InnerRadius * 2f,
                        0f,
                        CellV2.OuterRadius + y * (CellV2.OuterRadius * 1.5f)
                    );
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}