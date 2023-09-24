using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Uses/SwingHammer", fileName = "SwingHammer")]
public class SwingHammerUse : ItemUseConfig
{
	public int range = 1;

	public AnimationClip cwSwingClip;
	public AnimationClip ccwSwingClip;

	public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit, Item item)
	{
		return origin.GetCardinalRing(range).Where(t => Board.GetUnitAtPos(t) == null).ToList();
	}
}
