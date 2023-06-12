using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cell))]
public class CellEditor : Editor
{
    SerializedProperty surfaceFlagsProp;
    SerializedProperty pivotProp;

    private void OnEnable()
	{
        surfaceFlagsProp = serializedObject.FindProperty("surfaceFlags");
        pivotProp = serializedObject.FindProperty("pivot");
    }

	private void OnDisable()
	{
		
	}

	//private void OnSceneGUI()
	//{
 //       Cell cell = target as Cell;
 //       Vector2Int offsetCoord = Board.WorldToOffset(cell.transform.position);
 //       cell.transform.position.DrawString($"{offsetCoord.x}, {offsetCoord.y}", Color.green, -40f);
 //   }

	public override void OnInspectorGUI()
    {
        serializedObject.DrawScriptField();

        serializedObject.Update();

        DrawProperties();

        GUILayout.Space(20);

        DrawCoords();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawProperties()
	{
        EditorGUILayout.LabelField("MARKUP: ", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(pivotProp);
        EditorGUILayout.PropertyField(surfaceFlagsProp);
    }

    void DrawCoords()
	{
        Cell cell = (Cell)target;

        EditorGUILayout.LabelField("COORDS: ", EditorStyles.boldLabel);
        float3 cartesian = new float3(cell.transform.position.x, cell.transform.position.z, 1f);

        // Axial coordinates
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

        // Offset coordinates
        Vector2Int offset = Board.WorldToOffset(cell.transform.position);
        //Vector2Int offset = new Vector2Int(
        //    axial.x + (axial.y - (axial.y & 1)) / 2,
        //    axial.y
        //);

        EditorGUI.BeginChangeCheck();
        offset = EditorGUILayout.Vector2IntField("Global offset coords", offset);
        if (EditorGUI.EndChangeCheck())
        {
            // ^ Offset coordinates changed
            Vector2Int newAxial = new Vector2Int(
                offset.x - (offset.y - (offset.y & 1)) / 2,
                offset.y
            );
            float2 newCartesian = math.mul(Board.AxialToCartesian, new float3(newAxial.x, newAxial.y, 1f)).xy;
            Undo.RecordObject(cell.transform, "Changed cell position");
            cell.transform.position = new Vector3(newCartesian.x, cell.transform.position.y, newCartesian.y);
        }
    }
}