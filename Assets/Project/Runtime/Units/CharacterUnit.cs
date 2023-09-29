using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUnit : Unit
{
	//TODO: ... move these to character unit...

	public Dictionary<HolsteredStyle, Item> holsteredItemLookup = new Dictionary<HolsteredStyle, Item>();

	public Dictionary<EquippedStyle, Item> equippedItemLookup = new Dictionary<EquippedStyle, Item>();

	public Dictionary<HolsteredStyle, QuickBone> styleToHolster = new Dictionary<HolsteredStyle, QuickBone>();
	
	public Dictionary<BoneType, QuickBone> boneLookup = new Dictionary<BoneType, QuickBone>();

	public override void Start()
	{
		base.Start();

		boneLookup.Clear();

		var allBones = GetComponentsInChildren<QuickBone>();
		foreach(var bone in allBones)
		{
			if (boneLookup.ContainsKey(bone.type))
			{
				Debug.LogWarning($"already have a bone of type {bone.type.ToString()}", this.gameObject);
				continue;
			}

			boneLookup.Add(bone.type, bone);

			if (bone.type == BoneType.HIPHOLSTER)
				styleToHolster.Add(HolsteredStyle.HIP, bone);

			if (bone.type == BoneType.BACKHOLSTER)
				styleToHolster.Add(HolsteredStyle.BACK, bone);
		}

		if (!styleToHolster.ContainsKey(HolsteredStyle.HIP) || !styleToHolster.ContainsKey(HolsteredStyle.BACK))
		{
			Debug.LogWarning("character missing holster bones.", this.gameObject);
			return;
		}

		inventory.Clear();

		//Debug.LogWarning("Unit facing: " + Facing.ToString());

		foreach(var item in startingInventory)
		{
			var itemGameObject = Instantiate(
				item,
				this.transform.position,
				Quaternion.LookRotation(Facing.ToVector())
				);

			var itemInstance = itemGameObject.GetComponent<Item>();
			itemInstance.gameObject.name = item.name.ToUpper() + " - instance";

			itemInstance.name = item.name;
			itemInstance.HolsterTo(this);
		}

	}

	void LateUpdate()
	{
		foreach(var kvp in equippedItemLookup)
		{
			var item = kvp.Value;

			switch (item.equippedStyle)
			{
				case EquippedStyle.HAND:
					break;
				case EquippedStyle.HEAVY:
					break;
				case EquippedStyle.BACK:
					break;
				default:
					break;
			}
		}
	}
}
