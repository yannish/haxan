using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitiesV2/Tomahawk", fileName = "Tomahawk")]
public class TomahawkV2 : AbilityV2
{
	public int range;

	public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit)
	{
		return origin.GetCardinalCross(range, 1, HexOcclusion.INCLUSIVE);
	}
}
