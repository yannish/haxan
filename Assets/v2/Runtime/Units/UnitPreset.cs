using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellObjectPreset", menuName = "Units/UnitPreset")]
public class UnitPreset : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public bool isPassable;

    public bool isOccluding;

    public KnockResistance knockResistance;

    public UnitDeflectionProfile deflectionProfile;
}
