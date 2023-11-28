using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardUI))]
public class BoardUIEditor : Editor
{
    bool showCommands;
    bool showLookup;
    bool showTurns;
    bool showTurnSteps;
    bool showUnitOps;

    BoardUI boardUI;

    private void OnEnable() => EditorApplication.update += Repaint;

	private void OnDisable() => EditorApplication.update -= Repaint;

    public override void OnInspectorGUI()
	{
        boardUI = target as BoardUI;
        if (boardUI == null)
            return;

        //DrawCellMarkerLookup();
        //DrawCommandHistory();
        DrawDefaultInspector();
        EditorGUILayout.Space(20);
        DrawUnitOps();
	}

	private void DrawUnitOps()
	{
        if (!Application.isPlaying)
            return;

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("TURNS:", EditorStyles.boldLabel);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("SHOW"))
                {
                    showUnitOps = !showUnitOps;
                    Debug.LogWarning($"Toggled show unit ops: {showUnitOps}");
                }
            };
   
            if (!showUnitOps)
                return;

			for (int j = 0; j < Haxan.history.turnCount; j++)
			{
                Turn turn = Haxan.history.turns[j];

                using(new GUILayout.VerticalScope(EditorStyles.helpBox))
				{
					EditorGUILayout.LabelField($"... AT INDEX: {j}", EditorStyles.boldLabel);
					for (int k = turn.stepIndex; k < turn.stepIndex + turn.stepCount; k++)
                    {
                        TurnStep turnStep = Haxan.history.turnSteps[k];
                        for (int i = turnStep.opIndex; i < turnStep.opIndex + turnStep.opCount; i++)
                        {
                            UnitOp op = Haxan.history.allOps[i];
                            if (op == null)
                                continue;
                            op.DrawInspectorContent();
                        }
                    }
                }

				GUILayout.Space(20);
			}

         //   for (int i = 0; i < boardUI.totalOps; i++)
	        //{
         //       if (boardUI.allOps[i] == null)
         //           continue;
         //       boardUI.allOps[i].DrawInspectorContent();
	        //}
        }
    }
    
	void DrawCellMarkerLookup()
	{
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("CELL MARKER LOOKUP:", EditorStyles.boldLabel);
            showLookup = EditorGUILayout.Foldout(showLookup, "show", true);
            if (!showLookup)
                return;

            foreach (var kvp in boardUI.coordToCellMarkerLookup)
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
	}

 //   void DrawCommandHistory()
	//{
 //       GUI.enabled = false;

 //       using (new GUILayout.VerticalScope(EditorStyles.helpBox))
 //       {
 //           EditorGUILayout.LabelField("COMMAND HISTORY: ", EditorStyles.boldLabel);
 //           showCommands = EditorGUILayout.Foldout(showCommands, "show", true);
 //           if (showCommands)
 //           {
 //               if (GUILayout.Button("UNDO"))
 //                   boardUI.Undo();
 //               //DrawCommandHistory();
 //           }
 //       }


 //       if (!boardUI.turnHistory.IsNullOrEmpty())
	//	{
 //           foreach (var turn in boardUI.turnHistory)
 //           {
 //               //EditorGUILayout.BeginHorizontal();
 //               EditorGUILayout.ObjectField("instigator: ", turn.instigator, typeof(Unit), true);
 //               EditorGUI.indentLevel++;
 //               foreach(var command in turn.commandHistory)
	//			{
 //                   command.DrawInspectorContent();
 //                   EditorGUILayout.Space(5);
	//			}
 //               EditorGUI.indentLevel--;
 //           }
 //       }
 //       else
 //       {
 //           EditorGUILayout.LabelField("... no turns to display!");
 //       }
 //       GUI.enabled = true;
	//}
}
