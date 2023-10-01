using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditPrefabAssetScope : IDisposable
{
	public readonly string assetPath;
	public readonly GameObject prefabRoot;

	public EditPrefabAssetScope(string assetPath)
	{
		this.assetPath = assetPath;
		prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
	}

	public void Dispose()
	{
		PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
		PrefabUtility.UnloadPrefabContents(prefabRoot);
	}
}

public class ViewPrefabAssetScope : IDisposable
{
	public readonly string assetPath;
	public readonly GameObject prefabRoot;

	public ViewPrefabAssetScope(string assetPath)
	{
		this.assetPath = assetPath;
		prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
	}

	public void Dispose()
	{
		PrefabUtility.UnloadPrefabContents(prefabRoot);
	}
}

public class AssetHandler
{
    [MenuItem("Haxan/Update Unit asset paths")]
	public static void UpdateUnitAssetPaths()
	{
		string[] guids = AssetDatabase.FindAssets("t:prefab", new[] { "Assets/Resources" });
		Debug.Log($"Found {guids.Length} prefabs to check.");

		List<Unit> foundUnits = new List<Unit>();
		foreach(string guid in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			Unit foundUnit = AssetDatabase.LoadAssetAtPath<Unit>(path);
			if (foundUnit == null)
				continue;

			//if(!PrefabUtility.IsPartOfVariantPrefab(foundUnit.gameObject))
			//{
			//	Debug.LogWarning($"... skipping {foundUnit.gameObject.name}, isn't a variant");
			//	using (var editScope = new EditPrefabAssetScope(path))
			//	{
			//		var unit = editScope.prefabRoot.GetComponent<Unit>();
			//		if (unit != null)
			//		{
			//			unit.templatePath = "... not a variant, shoudldn't be used directly.";
			//			EditorUtility.SetDirty(editScope.prefabRoot);
			//		}
			//	}
			//	continue;
			//}

			foundUnits.Add(foundUnit);

			using (var editScope = new EditPrefabAssetScope(path))
			{
				var unit = editScope.prefabRoot.GetComponent<Unit>();
				if(unit != null)
				{
					string clippedString = path.Replace("Assets/", "");
					clippedString = clippedString.Replace("Resources/","");
					clippedString = clippedString.Replace(".prefab","");
					unit.templatePath = clippedString;
					EditorUtility.SetDirty(editScope.prefabRoot);
				}
			}
		}

		AssetDatabase.SaveAssets();

		Debug.Log($"Found {foundUnits.Count} units to modify.");
		//foreach (var unit in foundUnits)
		//{
			
		//}
	}
}
