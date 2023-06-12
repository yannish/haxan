using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    //public ItemConfig config;

	[Header("CONFIG:")]
	public Sprite icon;
	public HolsteredStyle holsteredStyle;
	public bool requiresReadying = false;
	public List<ItemUseConfig> uses = new List<ItemUseConfig>();


	[Header("STATE:")]
	public bool isReadied;

	[Header("BITS:")]
	public Transform _root;
	public Transform root
	{
		get
		{
			if (_root == null)
				_root = transform.Find("root");
			return _root;
		}
	}

	private void Awake()
	{
		_root = transform.Find("root");
	}
}

//public class ItemAction
//{
//	public Image icon;

//	//... from config:
//	public void ShowPreview(Vector2Int target, Unit unit) { }

//	public void HidePreview() { }
//}

public static class ItemExtentions
{
	public static void EquipTo(this Item item, CharacterUnit unit)
	{
		if(unit.equippedItemLookup.TryGetValue(item.holsteredStyle, out var foundItem))
		{
			Debug.LogWarning(
				$"Already have {foundItem.name} " +
				$"equipped to {item.holsteredStyle.ToString()} " +
				$"on {unit.name}"
				);
			return;
		}

		item.transform.position = unit.transform.position;

		if(unit.styleToHolster.TryGetValue(item.holsteredStyle, out var foundBone))
		{
			item.HolsterTo(foundBone);
		}

		unit.Items.Add(item);
	}

	public static void HolsterTo(this Item item, Transform holsterBone)
	{
		item.root.position = holsterBone.position;
		item.root.rotation = holsterBone.rotation;
		Debug.LogWarning("holstering to bone pos: " + holsterBone.position, holsterBone);
		item.transform.SetParent(holsterBone, true);
	}
}