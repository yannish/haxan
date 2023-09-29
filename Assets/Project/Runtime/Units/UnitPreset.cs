using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitPreset", menuName = "Units/UnitPreset")]
public class UnitPreset : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public GameObject prefab;

    public bool isPassable;

    public bool isOccluding;

    public KnockResistance knockResistance;

    public UnitDeflectionProfile deflectionProfile;
}
