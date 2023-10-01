using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scanned for & added to a lookup by ResourceManager.
/// Need to include a prefab reference for loading in.
/// </summary>
[CreateAssetMenu(fileName = "UnitArchetype", menuName = "Units/Unit Archetype")]
public class UnitDefinition : ScriptableObject
{
    public UnitType type;
    public GameObject prefab;
}
