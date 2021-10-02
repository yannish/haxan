using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Node), true)]
public class NodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var node = target as Node;

        serializedObject.Update();
        if (DrawNodeTypeSwitcher(node, serializedObject))
            return;
        serializedObject.ApplyModifiedProperties();
        DrawDefaultInspector();
    }

    private static bool DrawNodeTypeSwitcher(Node node, SerializedObject so)
    {
        var nodeTypes = typeof(Node).GetSubclassTypes();
        var nodeTypeStrings = nodeTypes.Select(t => t.ToString()).ToArray();

        var newSelectedNodeType = EditorGUILayout.Popup(
            "Node type: ",
            typeof(Node).GetSubclassEnumFromType(),
            nodeTypeStrings
            );

        if(GUI.changed)
        {
            var typeToAdd = typeof(Node).GetSubclassTypeFromEnum(newSelectedNodeType);
            var newNode = node.gameObject.AddComponent(typeToAdd) as Node;

            DestroyImmediate(node);
            newNode.gameObject.name = nodeTypeStrings[newSelectedNodeType];

            return true;
        }

        return false;
    }

    public Type[] GetNodeTypes()
    {
        var nodeTypes = typeof(Node).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Node)) && !t.IsAbstract)
            .ToArray();

        return nodeTypes;
    }

    public int GetNodeEnumFromType()//this Node node)
    {
        var nodeTypes = GetNodeTypes().ToList();

        int rtrn = 0;

        foreach (Type nodeType in nodeTypes)
            if (this.GetType() == nodeType)
                rtrn = nodeTypes.IndexOf(nodeType);

        return rtrn;
    }
}
