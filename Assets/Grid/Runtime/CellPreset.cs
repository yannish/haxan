using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellPreset", menuName = "Grids/CellPreset")]
public class CellPreset : ScriptableObject
{
	//[AssetList(Path = "/Config/Cell/Materials/")]
	public List<Material> baseMaterials;

	//[AssetList(Path = "/Config/Cell/")]
	public List<CellConfig> configs;

	//[AssetList(Path = "/Config/Cell/", Tags = "Prop")]
	public List<GameObject> baseProps;
}
