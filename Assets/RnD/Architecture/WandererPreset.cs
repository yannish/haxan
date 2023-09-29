using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scanned for & added to a lookup by ResourceManager.
/// Need to include a prefab reference for loading in.
/// </summary>
[CreateAssetMenu(fileName = "UnitArchetype", menuName = "Units/Unit Archetype")]
public class UnitReference : ScriptableObject
{
    public GameObject prefab;
	public UnitType type;

    //[Header("WANDERER:")]
    //public 
}

//public enum WandererType
//{
//    SCALLIWAG,
//    VETERAN,
//    ROOK
//}

public enum UnitType
{
	//... WANDERERS:
	SCALLIWAG,
	VETERAN,
	ROOK,

	//... ENEMIES:
	GHOL,
	WARREN,
	SCAVENGER,

	//... PROPS:
	CRATE,
	BARREL,
	PILLAR
}
