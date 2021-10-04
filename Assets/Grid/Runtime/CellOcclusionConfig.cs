using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0649 // never assigned.


[CreateAssetMenu(fileName = "CellConfig", menuName = "Grids/CellConfig/CellOccluding")]
public class CellOcclusionConfig : CellConfig, IOcclusionHandler
{
	[SerializeField] bool isOccluding;
	public bool IsOccluding { get { return isOccluding; } }
}
