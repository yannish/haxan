using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    public CellSwatch swatch;
	public GameObject brushPrefab;

	//public Cell_OLD cell;

    public EditorButton spawnBtn = new EditorButton("Spawn", false);
    public void Spawn()
	{
		var brushInstance = PrefabUtility.InstantiatePrefab(brushPrefab, this.transform) as GameObject;
		if (brushInstance == null)
			Debug.LogWarning("null brush instance");

		//brushInstance.transform.rotation = Quaternion.AngleAxis(
		//	UnityEngine.Random.Range(0, 6),
		//	Vector3.up
		//	);

		//var strokeToInstantiate = swatch.strokes[UnityEngine.Random.Range(0, swatch.strokes.Count)];
		//if (strokeToInstantiate == null)
		//	return;

		//var brushStrokeInstance = PrefabUtility.InstantiatePrefab(
		//	strokeToInstantiate,
		//	this.transform
		//	);

		//if (brushStrokeInstance == null)
		//	return;

		//var gameObj = (GameObject)brushStrokeInstance;
		//if (gameObj == null)
		//	return;

		//gameObj.transform.rotation = Quaternion.AngleAxis(
		//	UnityEngine.Random.Range(0, 6),
		//	Vector3.up
		//	);
	}
}
