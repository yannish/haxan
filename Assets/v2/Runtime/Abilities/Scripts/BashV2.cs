using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitiesV2/Bash", fileName = "Bash")]
public class BashV2 : AbilityV2
{
	public override List<Vector2Int> GetValidMoves(Vector2Int origin, Unit unit)
	{
		return origin.GetCardinalRing(1);
	}
}
