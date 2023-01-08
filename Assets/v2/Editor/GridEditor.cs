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

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(numTiles);
        if (EditorGUI.EndChangeCheck())
        {
        }
        serializedObject.ApplyModifiedProperties();
    }
}