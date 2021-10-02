using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CellPainter : MonoBehaviour
{
	public CellPreset currentPaint;
	public List<CellPreset> presets = new List<CellPreset>();

	public static void PaintCell(Cell cell, CellPreset cellPreset)
	{
		Debug.Log("PAINTING CELL!");

		cell.preset = cellPreset;

		if (!cellPreset.baseMaterials.IsNullOrEmpty())
		{
			foreach(var mat in cellPreset.baseMaterials)
			{
				//Debug.Log("... mat: " + mat.name);
			}

			var index = UnityEngine.Random.Range(0, cellPreset.baseMaterials.Count);
			Debug.Log("... selected index: " + index);

			var randomMat = cellPreset.baseMaterials[index];
			cell.baseMeshRenderer.sharedMaterial = randomMat;
		}

		if (cell.TryGetBoundCellObject(out CellObject foundObject))
		{
			DestroyImmediate(foundObject);
		}

		if (!cellPreset.baseProps.IsNullOrEmpty())
		{
			Debug.Log("instantiating: " + cellPreset.baseProps[0].name);
			var newPrefab = PrefabUtility.InstantiatePrefab(cellPreset.baseProps[0]);
			//var newPrefab = PrefabUtility.InstantiatePrefab(cellPreset.baseProps[0] as GameObject);

			var newCellObj = (newPrefab as GameObject).GetComponent<CellObject>();

			if (newCellObj != null)
				Debug.Log("it's a game object");
			else
				Debug.Log("it's not a game object");

			//var newCellObj = (
			//	PrefabUtility.InstantiatePrefab(
			//		cellPreset.baseProps[UnityEngine.Random.Range(0, cellPreset.baseProps.Count)]
			//		) 
			//		as GameObject)
			//		.GetComponent<CellObject>();

			//var cellObject = cellPreset.baseProps[0];
			////var cellObject = cellPreset.baseProps.Random();
			//var newPrefab = PrefabUtility.InstantiatePrefab(cellObject) as GameObject;

			////GameObject newPrefabObj = (GameObject)newPrefab;

			//var newCellObject = newPrefab.GetComponent<CellObject>();

			newCellObj.MoveAndBindTo(cell);
		}

		PrefabUtility.RecordPrefabInstancePropertyModifications(cell);
		EditorSceneManager.MarkSceneDirty(cell.gameObject.scene);
		EditorUtility.SetDirty(cell);

		SceneView.RepaintAll();
	}
}
