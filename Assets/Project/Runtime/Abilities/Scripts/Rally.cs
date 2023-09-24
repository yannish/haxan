using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitiesV2/Rally", fileName = "Rally")]
public class Rally : Ability
{
	public int range;
	public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit)
	{
		return origin.GetCellsInRadius(range);
	}
}

