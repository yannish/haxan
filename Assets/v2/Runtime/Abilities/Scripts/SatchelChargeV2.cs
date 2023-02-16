using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitiesV2/SatchelChargeV2", fileName = "SatchelChargeV2")]
public class SatchelChargeV2 : AbilityV2
{
	public int range;

	public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit)
	{
		return origin.GetCellsInRadius(range);
	}
}
