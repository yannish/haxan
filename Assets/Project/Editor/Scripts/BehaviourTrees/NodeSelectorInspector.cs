using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeSelector))]
public class NodeSelectorInspector : Editor
{
    private NodeSelector nodeSelector;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        nodeSelector = target as NodeSelector;

        if (GUILayout.Button("Log types"))
            LogTypes();
    }

    void LogTypes()
    {
        Debug.Log(nodeSelector.node.GetType().ToString());
        Debug.Log(nodeSelector.node.GetType().BaseType.ToString());
    }
}
