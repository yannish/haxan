using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellObjectPreset", menuName = "Units/UnitPreset")]
public class UnitPreset : ScriptableObject
{
    public bool isPassable;

    public bool isOccluding;

    public KnockResistance knockResistance;

    public UnitDeflectionProfile deflectionProfile;
}
