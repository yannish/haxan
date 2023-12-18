
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class QuickBoardState
{
	public string stringA;
	public BoardHistory history;
}

public class SaveUI : MonoBehaviour
{
	const string saveFolder = "/saves/";
	const string debugSaveFolder = "/debugSaveFolder/";

	public BoardState capturedBoardState;

	public string[] existingSaves;
	public List<string> existingSaveNames = new List<string>();


	[Header("PREFABS:")]
	public GameObject saveFileButtonPrefab;
	public GameObject deleteButtonPrefab;
	public GameObject overwriteButtonPrefab;


	[Header("DEBUG:")]
	public string[] debugExistingFiles;
	public string debugFileName;
	public string debugFilePath;
	public int debugFilePathIndex;
	public BoardState debugBoardState;



	public EditorButton debugLoadBtn = new EditorButton("DebugLoad");
	public void DebugLoad()
	{
		if (!File.Exists(debugExistingFiles[0]))
		{
			Debug.LogWarning($"No file found at {debugExistingFiles[0]}.");
			return;
		}

		Debug.LogWarning($"Loading from {debugExistingFiles[0]}.");

		var boardStateData = System.IO.File.ReadAllText(debugExistingFiles[0]);
		quickBoardStateB = JsonUtility.FromJson<QuickBoardState>(boardStateData);
	}

	public EditorButton debugSaveBtn = new EditorButton("DebugSave");
	public void DebugSave()
	{
		if (!Directory.Exists(Application.persistentDataPath + debugSaveFolder))
			Directory.CreateDirectory(Application.persistentDataPath + debugSaveFolder);

		string boardStateString = JsonUtility.ToJson(quickBoardStateA);
		string filePath = Application.persistentDataPath + debugSaveFolder + $"{debugFileName}.json";
		System.IO.File.WriteAllText(filePath, boardStateString);
		debugExistingFiles = Directory.GetFiles(Application.persistentDataPath + debugSaveFolder);
	}

	public EditorButton debugClearBtn = new EditorButton("DebugClear");
	public void DebugClear()
	{
		debugExistingFiles = Directory.GetFiles(Application.persistentDataPath + debugSaveFolder);
		foreach (var savePath in debugExistingFiles)
		{
			Debug.LogWarning($"deleting at {savePath}");
			if (File.Exists(savePath))
				File.Delete(savePath);
		}
		debugExistingFiles = null;
	}

	public QuickBoardState quickBoardStateA;
	public QuickBoardState quickBoardStateB;





	[Header("REFERENCES:")]
	public Button saveButton;
	public Button cancelButton;
	public TMP_InputField inputField;

	Canvas canvas;
	public VerticalLayoutGroup loadButtonGroup;
	public VerticalLayoutGroup deleteButtonGroup;
	public VerticalLayoutGroup overwriteButtonGroup;
	public List<Button> loadFileButtons = new List<Button>();
	public List<Button> deleteFileButtons = new List<Button>();
	public List<Button> overwriteFileButtons = new List<Button>();
	bool isShowing;


	//... BUTTONS:
	//public EditorButton saveBtn = new EditorButton("Save");
	//public void Save()
	//{
	//	SaveInternal(saveGameName);
	//}

	//public EditorButton loadbBtn = new EditorButton("Load");
	//public void Load()
	//{
	//	LoadInternal(Application.persistentDataPath + saveFolder + $"{saveGameName}.json");
	//}

	public EditorButton trimNamesBtn = new EditorButton("TrimNames");
	public void TrimNames()
	{
		existingSaveNames.Clear();
		foreach (var existingSave in existingSaves)
		{
			var fileName = Path.GetFileName(existingSave);
			existingSaveNames.Add(fileName);
		}
	}

	public EditorButton clearBtn = new EditorButton("ClearLoadedThing");
	public void ClearLoadedThing() => Haxan.stateVariable.state = null;

	public EditorButton refreshSaveStrings = new EditorButton("RefreshSaveStrings");
	void RefreshSaveStrings() => existingSaves = Directory.GetFiles(Application.persistentDataPath + saveFolder);

	public EditorButton deleteAll = new EditorButton("DeleteAll");
	public void DeleteAll()
	{
		foreach (var savePath in existingSaves)
		{
			Debug.LogWarning($"deleting at {savePath}");
			if (File.Exists(savePath))
				File.Delete(savePath);
		}
	}


	private void Awake()
	{
		canvas = GetComponentInParent<Canvas>();
		loadFileButtons = loadButtonGroup.gameObject.GetComponentsInChildren<Button>().ToList();
		deleteFileButtons = deleteButtonGroup.gameObject.GetComponentsInChildren<Button>().ToList();
		overwriteFileButtons = overwriteButtonGroup.gameObject.GetComponentsInChildren<Button>().ToList();

		HideMenu();

		saveButton.onClick.AddListener(() => OnSaveButtonClicked());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleWindow();
		}
	}


	void ToggleWindow()
	{
		if (!isShowing)
			ShowMenu();
		else
			HideMenu();
	}

	void ShowMenu()
	{
		isShowing = true;
		canvas.enabled = true;

		GameContext.state = GameFlowState.PAUSED;

		RefreshButtons();
	}

	void HideMenu()
	{
		isShowing = false;
		canvas.enabled = false;

		GameContext.state = GameFlowState.RUNNING;

		ClearButtons();
	}

	void RefreshButtons()
	{
		Debug.LogWarning("REFRESHING BUTTONS.");

		ClearButtons();
		CheckForFolder();
		RefreshSaveStrings();
		TrimNames();
		GenerateButtons();
	}

	void CheckForFolder()
	{
		if (!Directory.Exists(Application.persistentDataPath + saveFolder))
			Directory.CreateDirectory(Application.persistentDataPath + saveFolder);
	}

	void GenerateButtons()
	{
		for (int i = 0; i < existingSaves.Length; i++)
		{
			CreateLoadButton(i);
			CreateDeleteButton(i);
			CreateOverwriteButton(i);
		}
	}

	void ClearButtons()
	{
		foreach (var button in loadFileButtons)
			Destroy(button.gameObject);
		loadFileButtons.Clear();

		foreach (var button in deleteFileButtons)
			Destroy(button.gameObject);
		deleteFileButtons.Clear();

		foreach (var button in overwriteFileButtons)
			Destroy(button.gameObject);
		overwriteFileButtons.Clear();
	}

	void CreateLoadButton(int index)
	{
		var newLoadButtonObj = Instantiate(saveFileButtonPrefab, loadButtonGroup.transform);
		var newLoadButton = newLoadButtonObj.GetComponentInChildren<Button>();
		var buttonText = newLoadButtonObj.GetComponentInChildren<TextMeshProUGUI>();

		buttonText.SetText(existingSaveNames[index]);
		loadFileButtons.Add(newLoadButton);

		newLoadButton.onClick.AddListener(() =>
		{
			OnLoadButtonClicked(index);
			RefreshButtons();
		});
	}

	void CreateDeleteButton(int index)
	{
		var newButtonObj = Instantiate(deleteButtonPrefab, deleteButtonGroup.transform);
		var newButton = newButtonObj.GetComponentInChildren<Button>();

		deleteFileButtons.Add(newButton);

		newButton.onClick.AddListener(() =>
		{
			OnDeleteButtonClicked(index);
			RefreshButtons();
		});
	}

	void CreateOverwriteButton(int index)
	{
		var newButtonObj = Instantiate(overwriteButtonPrefab, overwriteButtonGroup.transform);
		var newButton = newButtonObj.GetComponentInChildren<Button>();

		overwriteFileButtons.Add(newButton);

		newButton.onClick.AddListener(() =>
		{
			OnOverwriteButtonClicked(index);
			RefreshButtons();
		});
	}

	void OnOverwriteButtonClicked(int index)
	{
		Debug.LogWarning($"CLICKED OVERWRITE: {existingSaveNames[index]}");
		OverwriteInternal(index);
	}

	void OverwriteInternal(int index)
	{
		SaveInternal(existingSaveNames[index]);
	}

	void OnDeleteButtonClicked(int index)
	{
		Debug.LogWarning($"CLICKED BUTTON: {existingSaveNames[index]}");
		DeleteInternal(index);
	}

	void DeleteInternal(int index)
	{
		if (File.Exists(existingSaves[index]))
			File.Delete(existingSaves[index]);
	}

	void OnLoadButtonClicked(int index)
	{
		Debug.LogWarning($"CLICKED BUTTON: {existingSaveNames[index]}");
		LoadInternal(existingSaves[index]);
	}

	void LoadInternal(string path)
	{
		if (!File.Exists(path))
		{
			Debug.LogWarning($"No file found at {path}.");
			return;
		}

		Debug.LogWarning($"Loading from {path}.");

		GameContext.OnLoadBoardStateBegin?.Invoke();

		var boardStateData = System.IO.File.ReadAllText(path);
		Haxan.stateVariable.state = JsonUtility.FromJson<BoardState>(boardStateData);

		GameContext.OnLoadBoardStateComplete?.Invoke();
	}

	void OnSaveButtonClicked()
	{
		if (inputField.text == "")
		{
			Debug.LogWarning("... save file requires a name");
			return;
		}

		foreach (var existingSaveName in existingSaveNames)
		{
			if (existingSaveName == inputField.text)
			{
				Debug.LogWarning($"... already have a save game with name {existingSaveName}!");
				return;
			}
		}

		SaveInternal(inputField.text);
		RefreshButtons();
	}

	public BoardHistory capturedBoardHistory;
	void SaveInternal(string fileName)
	{
		if (!Directory.Exists(Application.persistentDataPath + saveFolder))
			Directory.CreateDirectory(Application.persistentDataPath + saveFolder);

		var capturedBoardLayout = new BoardLayout();
		foreach(var unit in Haxan.activeUnits.Items)
		{
			var cachedUnitState = unit.CacheState();
			capturedBoardLayout.unitStates.Add(cachedUnitState);
		}

		capturedBoardHistory = Haxan.history;

		capturedBoardState = new BoardState();
		capturedBoardState.history = capturedBoardHistory;
		capturedBoardState.layout = capturedBoardLayout;

		for (int i = 0; i < capturedBoardState.history.turnCount; i++)
		{
			Debug.LogWarning($"... {i}");
		}

		string boardStateString = JsonUtility.ToJson(capturedBoardState);
		string filePath = Application.persistentDataPath + saveFolder + $"{fileName}.json";
		System.IO.File.WriteAllText(filePath, boardStateString);
	}
}
