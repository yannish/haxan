using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KnockResistance
{
    LIGHT,
    HEAVY,
    IMMOVABLE
}

[CreateAssetMenu(fileName = "CellObjectPreset", menuName = "Grids/CellObjectPreset")]
public class CellObjectPreset : ScriptableObject
{
    public bool isPassable;
    
    public bool isOccluding;

    public KnockResistance knockResistance;

    public CellDeflectionProfile deflectionProfile;

    public List<CellObjectConfig> configs;
}
