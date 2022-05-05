using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandererDisplay : MonoBehaviour
{
	public GameObject wandererSlotPrefab;

	public void Start()
	{
		Debug.LogWarning("Making " + Globals.ActiveWanderers.Items.Count);

		foreach(var wanderer in Globals.ActiveWanderers.Items)
		{
			var newSlot = Instantiate(wandererSlotPrefab);
			newSlot.transform.SetParent(this.transform);

			var wandererSlot = newSlot.GetComponent<WandererDisplaySlot>();
			wandererSlot.BindTo(wanderer as Wanderer);
		}
	}
}
