using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitiesV2/Rally", fileName = "Rally")]
public class RallyV2 : AbilityV2
{
	public int range;
	public override List<Vector2Int> GetValidMoves(Vector2Int origin, Unit unit)
	{
		return origin.GetCellsInRadius(range);
	}
}

