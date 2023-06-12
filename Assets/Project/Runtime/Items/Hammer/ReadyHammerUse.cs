using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Uses/ReadyHammer", fileName = "ReadyHammer")]
public class ReadyHammerUse : ItemUseConfig
{
	public Sprite readyIcon;

	public Sprite unreadyIcon;

	public override bool IsReadyUse => true;

	public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit)
	{
		return origin.GetCardinalRing(1).Where(t => Board.GetUnitAtPos(t) == null).ToList();
	}

	public override void UpdateButton(ItemUseButton useButton, Item item)
	{
		useButton.icon.sprite = item.isReadied ? unreadyIcon : readyIcon;

		//if (item.isReadied)
		//{
			//useButton.icon.sprite = unreadyIcon;
		//}
	}
}
