using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Uses/SwingHammer", fileName = "SwingHammer")]
public class SwingHammerUse : ItemUseConfig
{
    public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit)
	{
		return origin.GetCardinalRing(1).Where(t => Board.GetUnitAtPos(t) == null).ToList();
	}
}
