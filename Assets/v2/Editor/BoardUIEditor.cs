using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardUI))]
public class BoardUIEditor : Editor
{
    bool showCommands;
    bool showLookup;

    BoardUI boardUI;

    private void OnEnable() => EditorApplication.update += Repaint;

	private void OnDisable() => EditorApplication.update -= Repaint;

    public override void OnInspectorGUI()
	{
        boardUI = target as BoardUI;
        if (boardUI == null)
            return;

        EditorGUILayout.LabelField("CELL MARKER LOOKUP:", EditorStyles.boldLabel);
        showLookup = EditorGUILayout.Foldout(showLookup, "show");
        if (showLookup)
        {
            DrawCellMarkerLookup();
        }

        EditorGUILayout.LabelField("COMMAND HISTORY: ", EditorStyles.boldLabel);
        showCommands = EditorGUILayout.Foldout(showCommands, "show");
		if (showCommands)
		{
			if (GUILayout.Button("UNDO"))
                boardUI.Undo();

			//DrawCommandHistory();
		}

        DrawDefaultInspector();
	}

    void DrawCellMarkerLookup()
	{
        foreach(var kvp in boardUI.coordToCellMarkerLookup)
		{
            var coord = kvp.Key;
            var marker = kvp.Value;

            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
			{
                EditorGUILayout.LabelField(coord.ToString(), EditorStyles.boldLabel);
                EditorGUILayout.ObjectField(marker, typeof(PooledCellVisuals), true);
			}
		}
	}

    void DrawCommandHistory()
	{
        GUI.enabled = false;

		if (!boardUI.turnHistory.IsNullOrEmpty())
		{
            foreach (var turn in boardUI.turnHistory)
            {
                //EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField("instigator: ", turn.instigator, typeof(Unit), true);
                EditorGUI.indentLevel++;
                foreach(var command in turn.commandHistory)
				{
                    command.DrawInspectorContent();
                    EditorGUILayout.Space(5);
				}
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUILayout.LabelField("... no turns to display!");
        }
        GUI.enabled = true;
	}
}
