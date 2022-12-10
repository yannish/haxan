using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ExposedScriptableObjectAttribute))]
public class ExposedScriptableObjectAttributeDrawer : PropertyDrawer
{
    private Editor editor = null;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Draw label
		EditorGUI.PropertyField(position, property, label, true);

		// Draw foldout
		if (property.objectReferenceValue != null)
		{
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
		}

		if(property.isExpanded)
		{
			EditorGUI.indentLevel++;

			// Draw object properties
			if (!editor)
				Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
			editor.OnInspectorGUI();

			EditorGUI.indentLevel--;
		}
 	}
}
