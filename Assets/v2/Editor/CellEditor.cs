using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellV2))]
public class CellEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CellV2 cell = (CellV2)target;
        float3 cartesian = new float3(cell.transform.position.x, cell.transform.position.z, 1f);
        // Axial coordinates
        {
            float2 axialFrac = math.mul(Board.CartesianToAxial, cartesian).xy;
            Vector2Int axial = new Vector2Int(Mathf.RoundToInt(axialFrac.x), Mathf.RoundToInt(axialFrac.y));
            EditorGUI.BeginChangeCheck();
            axial = EditorGUILayout.Vector2IntField("Global axial coords", axial);
            if (EditorGUI.EndChangeCheck())
            {
                // ^ Axial coordinates changed
                float2 newCartesian = math.mul(Board.AxialToCartesian, new float3(axial.x, axial.y, 1f)).xy;
                Undo.RecordObject(cell.transform, "Changed cell position");
                cell.transform.position = new Vector3(newCartesian.x, cell.transform.position.y, newCartesian.y);
            }
        }
    }
}