using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUnit : Unit
{
	//TODO: ... move these to character unit...

	[ReadOnly] public List<Transform> holsters = new List<Transform>();

	public Dictionary<HolsteredStyle, Transform> styleToHolster = new Dictionary<HolsteredStyle, Transform>();

	public Dictionary<HolsteredStyle, Item> equippedItemLookup = new Dictionary<HolsteredStyle, Item>();

	public override void Start()
	{
		base.Start();

		Transform hipHolster = null;
		Transform backHolster = null;

		var allBones = GetComponentsInChildren<QuickBone>();
		foreach(var bone in allBones)
		{
			if (bone.name == "hipHolster")
				hipHolster = bone.transform;

			if (bone.name == "backHolster")
				backHolster = bone.transform;

			if (backHolster != null && hipHolster != null)
				break;
		}

		//var hipHolster = this.transform.Find("hipHolster");
		//var backHolster = this.transform.Find("backHolster");

		if (hipHolster == null || backHolster == null)
		{
			Debug.LogWarning("character missing holster bones.", this.gameObject);
			return;
		}

		holsters.Add(hipHolster);
		holsters.Add(backHolster);

		styleToHolster.Add(HolsteredStyle.BACK, backHolster);
		styleToHolster.Add(HolsteredStyle.HIP, hipHolster);

		Items.Clear();

		Debug.LogWarning("Unit facing: " + Facing.ToString());

		foreach(var item in startingInventory)
		{
			var itemGameObject = Instantiate(
				item,
				this.transform.position,
				Quaternion.LookRotation(Facing.ToVector())
				//Quaternion.identity
				);

			var itemInstance = itemGameObject.GetComponent<Item>();
			itemInstance.gameObject.name = item.name.ToUpper() + " - instance";
			//if (itemInstance == null)
			//{
			//	Debug.LogWarning("NO ITEM.CS ON CONFIG: " + item.name);
			//}
			itemInstance.name = item.name;
			itemInstance.EquipTo(this);
		}
	}
}
