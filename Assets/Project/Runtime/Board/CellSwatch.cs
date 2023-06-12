using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cells/Swatch", fileName = "Swatch")]
public class CellSwatch : ScriptableObject
{
	public CellSurfaceFlags cellState;
	public List<CellBrushStroke> strokes = new List<CellBrushStroke>();
}
