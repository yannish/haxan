using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0649 // never assigned.

[CreateAssetMenu(fileName = "CellNavConfig", menuName = "Grids/CellConfig/CellNav")]
public class CellNavConfig : CellConfig, INavHandler
{
	[SerializeField] bool isNavigable;
	public bool IsNavigable { get { return isNavigable; } }
}
