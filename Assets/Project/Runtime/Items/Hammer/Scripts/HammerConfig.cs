using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Probably need to rename Item to ItemConfig
/// Create an instanced class "item" which exists in-scene & contains some local data (breakage, ammo, etc)
/// </summary>

[CreateAssetMenu(menuName = "Items/Hammer", fileName = "Hammer")]
public class HammerConfig : ItemConfig
{
    public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit)
	{
		return origin.GetCardinalRing(1).Where(t => Board.GetUnitAtPos(t) == null).ToList();
	}

	public override void OnEquip(Unit unit)
	{
		//var hammerInstance = Instantiate(propPrefab);
	}
}
