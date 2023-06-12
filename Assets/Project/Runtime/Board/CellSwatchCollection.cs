using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cells/SwatchCollection", fileName = "SwatchCollection")]
public class CellSwatchCollection : ScriptableObject
{
    //... a bunch of swatches, randomized & placed to mark some aspect of a cell's state
    public List<CellSwatch> swatches = new List<CellSwatch>();

    public CellSwatch selectedSwatch;
}
