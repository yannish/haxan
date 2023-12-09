using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DumbSaveableThing
{
	public DumbNestableThingA thingA;
	public DumbNestableThingB thingB;
}

[Serializable]
public class DumbNestableThingA
{
	public float quickFloat;
	public string quickString;
}

[Serializable]
public class DumbNestableThingB
{
	public int quickInt;
	public bool quickBool;
}


public class SaveTestRunner : MonoBehaviour
{
	const string saveFolder = "/testSaves/";

	public string saveGameName;

	public string[] existingSaves;

	public GameObject buttonPrefab;

	public DumbSaveableThing thingToSave;
	public DumbSaveableThing loadedThing;

	public EditorButton saveBtn = new EditorButton("Save");
	public void Save()
	{
		if(!Directory.Exists(Application.persistentDataPath + saveFolder))
		{
			Directory.CreateDirectory(Application.persistentDataPath + saveFolder);
		}

		string saveableThingData = JsonUtility.ToJson(thingToSave);
		string filePath = Application.persistentDataPath + saveFolder + $"{saveGameName}.json";
		System.IO.File.WriteAllText(filePath, saveableThingData);

		//existingSaves = Directory.GetFiles(Application.persistentDataPath + saveFolder);
	}

	public EditorButton loadbBtn = new EditorButton("Load");
	public void Load()
	{
		LoadInternal(Application.persistentDataPath + saveFolder + $"{saveGameName}.json");
	}

	void LoadInternal(string path)
	{
		if (!File.Exists(path))
		{
			Debug.LogWarning($"No file found at {path}.");
			return;
		}

		var saveableThingData = System.IO.File.ReadAllText(path);
		loadedThing = JsonUtility.FromJson<DumbSaveableThing>(saveableThingData);
		//loadableThing = 
	}

	public EditorButton clearBtn = new EditorButton("Clear");
	public void Clear()
	{
		loadedThing = null;
	}

	public EditorButton loadAll = new EditorButton("LoadAll");
	public void LoadAll()
	{
		existingSaves = Directory.GetFiles(Application.persistentDataPath + saveFolder);
	}

	public EditorButton deleteAll = new EditorButton("DeleteAll");
	public void DeleteAll()
	{
		foreach (var savePath in existingSaves)
		{
			if(File.Exists(savePath))
				File.Delete(savePath);
		}
			//FileUtil.DeleteFileOrDirectory(save);
				//Directory.Delete(save);
	}


	Canvas canvas;
	VerticalLayoutGroup buttonGroup;
	List<Button> buttons = new List<Button>();
	private void Awake()
	{
		canvas = GetComponentInParent<Canvas>();
		canvas.enabled = false;
		buttonGroup = GetComponentInChildren<VerticalLayoutGroup>();
		buttons = GetComponentsInChildren<Button>().ToList();
		HideButtons();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleWindow();
		}
	}

	bool isShowing;
	private void ToggleWindow()
	{
		if (!isShowing)
		{
			ShowButtons();
		}
		else
		{
			HideButtons();
		}
	}


	public void ShowButtons()
	{
		isShowing = true;

		canvas.enabled = true;
		ClearButtons();

		existingSaves = Directory.GetFiles(Application.persistentDataPath + saveFolder);
		for (int i = 0; i < existingSaves.Length; i++)
		{
			var fileName = Path.GetFileName(existingSaves[i]);
			CreateButton(fileName);
		}
	}

	private void HideButtons()
	{
		isShowing = false;
		canvas.enabled = false;
		ClearButtons();
	}

	void ClearButtons()
	{
		foreach (var button in buttons)
			GameObject.Destroy(button);
		buttons.Clear();
	}

	void CreateButton(string name)
	{
		var newButtonObj = Instantiate(buttonPrefab, buttonGroup.transform);
		var newButton = newButtonObj.GetComponentInChildren<Button>();
		var buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
		buttonText.SetText(name);
		buttons.Add(newButton);
		newButton.onClick.AddListener(() => OnButtonClicked(name));
	}

	void OnButtonClicked(string buttonFile)
	{
		Debug.LogWarning($"CLICKED BUTTON: {buttonFile}");
	}
}
