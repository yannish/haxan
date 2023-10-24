using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemUseConfig : ScriptableObject
{
    public Sprite icon;

	public virtual void ShowPreview(Vector2Int origin, Unit unit, Item item) { }

	public virtual void HidePreview(Vector2Int origin, Unit unit, Item item) { }

    public virtual List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit, Item item) => null;

	public virtual Queue<UnitCommandStep> FetchCommandStepChain(Vector2Int targetCoord, Unit unit) => null;


	public virtual bool IsReadyUse => false;

    public virtual void UpdateButton(ItemUseButton useButton, Item item)
	{
        if(item.requiresReadying)
		{
			if(item.isReadied)
			{
				useButton.SetAvailable();
			}
			else
			{
				useButton.SetUnavailable();
			}
		}
		else
		{
			useButton.SetAvailable();
		}
	}
}

//public class ItemReadyingUseConfig : ItemUseConfig
//{
//    public Sprite readyIcon;
//    public Sprite unreadyIcon;
//}

//public interface IReadying
//{
//    public Sprite FetchReadyIcon();
//    public Sprite FetchUnreadyIcon();
//}