using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using bogTools;
using BOG;

//public enum DebogSoloType
//{
//	None,
//	IMPORTANT,
//	COMBAT,
//	FIGHTERS,
//	GAMEFLOW,
//	INVENTORY,
//	TAGS,
//	CAMERA,
//	EDITOR,
//	ANIMATION,
//	GRID,
//	INPUT
//}

public class DebogPrefs : EditorWindow
{
	bool showMuteAll;

	bool showImportant;
	bool showCombat;
	bool showFighters;
	bool showGameflow;
	bool showInventory;
	bool showTags;
	bool showCamera;
	bool showEditor;
	bool showAnimation;
	bool showGrid;
	bool showInput;

	//bool soloImportant;
	//bool soloCombat;
	//bool soloFighters;
	//bool soloGameflow;
	//bool soloInventory;
	//bool soloTags;
	//bool soloCamera;
	//bool soloEditor;
	//bool soloAnimation;
	//bool soloGrid;
	//bool soloInput;

	[MenuItem("BOG/DebogPrefs")]
	static void Init()
	{
		DebogPrefs window = (DebogPrefs)EditorWindow.GetWindow(typeof(DebogPrefs));
		window.Show();
	}

	DebogConfig config;

	void OnGUI()
	{
		showMuteAll = EditorGUILayout.Toggle(
			"MUTE ALL",
			config.muteAll
			);

		showImportant = EditorGUILayout.Toggle(
			Debog.importantString,
			config.important
			);
		showCombat = EditorGUILayout.Toggle(
			Debog.combatString,
			config.combat
			);
		showFighters = EditorGUILayout.Toggle(
			Debog.fightersString,
			config.fighters
			);
		showGameflow = EditorGUILayout.Toggle(
			Debog.gameflowString,
			config.gameflow
			);
		showInventory = EditorGUILayout.Toggle(
			Debog.inventoryString,
			config.inventory
			);
		showTags = EditorGUILayout.Toggle(
			Debog.tagString,
			config.tags
			);
		showCamera = EditorGUILayout.Toggle(
			Debog.cameraString,
			config.camera
			);
		showInput = EditorGUILayout.Toggle(
			Debog.inputString,
			config.input
			);
		showAnimation = EditorGUILayout.Toggle(
			Debog.animationString,
			config.animation
			);
		showEditor = EditorGUILayout.Toggle(
			Debog.editorString,
			config.editor
			);


		
		if (GUI.changed)
		{
			EditorPrefs.SetBool("MUTE ALL", showMuteAll);
			EditorPrefs.SetBool(Debog.importantString, showImportant);
			EditorPrefs.SetBool(Debog.combatString, showCombat);
			EditorPrefs.SetBool(Debog.fightersString, showFighters);
			EditorPrefs.SetBool(Debog.gameflowString, showGameflow);
			EditorPrefs.SetBool(Debog.inventoryString, showInventory);
			EditorPrefs.SetBool(Debog.tagString, showTags);
			EditorPrefs.SetBool(Debog.cameraString, showCamera);
			EditorPrefs.SetBool(Debog.inputString, showInput);
			EditorPrefs.SetBool(Debog.animationString, showAnimation);
			EditorPrefs.SetBool(Debog.editorString, showEditor);

			config.muteAll = showMuteAll;
			config.important = showImportant;
			config.combat = showCombat;
			config.fighters = showFighters;
			config.gameflow = showGameflow;
			config.inventory = showInventory;
			config.tags = showTags;
			config.camera = showCamera;
			config.input = showInput;
			config.animation = showAnimation;
			config.editor = showEditor;

			EditorUtility.SetDirty(config);
			AssetDatabase.SaveAssets();
		}
	}

	private const string prefabPath = "Debog/debogConfig";
	void OnFocus()
	{
		if(config == null)
			config = Resources.Load(prefabPath, typeof(DebogConfig)) as DebogConfig;
	}

	void OnLostFocus() { }

	void OnDestroy() { }
}
