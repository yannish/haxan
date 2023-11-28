using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * For now: give items their own clip player. 
 * They'll animate suspended in space, not held by player.
 * 
 * For later: 
 * 1. animate the player
 * 2. attach weapons and gear to their rig
 * 3. inject the clips you need from the gear to the player.
*/

public enum OpInterruptType
{
	PASS,		//... no response
	RETALIATE,	//... respond after allowing op to execute
	INTERDICT	//... respond to prevent op from executing
}

[SerializeField]
public class MaterialCache
{
	public Renderer renderer;
	public Material[] materials;
}

public class Item : MonoBehaviour
{
	private const string hologramMatPath = "Materials/MAT hologram";

	[ReadOnly] public ClipHandler clipHandler;
	[ReadOnly] public Transform _root;
	[ReadOnly] public Material hologramMat;
	[ReadOnly] public Renderer[] renderers;
	private void OnValidate()
	{
		renderers = GetComponentsInChildren<Renderer>();
		materialCaches.Clear();
		foreach(var rend in renderers)
		{
			materialCaches.Add(new MaterialCache()
			{
				renderer = rend,
				materials = rend.sharedMaterials
			});
		}
	}

	public Transform root
	{
		get
		{
			if (_root == null)
				_root = transform.Find("root");
			return _root;
		}
	}

	[Header("CONFIG:")]
	public Sprite icon;
	public EquippedStyle equippedStyle;
	public HolsteredStyle holsteredStyle;
	public bool requiresReadying = false;
	public GameObject hologram;

	[Header("STATE:")]
	public bool isReadied;

	[Header("USES:")]
	public List<ItemUseConfig> allUses = new List<ItemUseConfig>();


	public virtual UnitCommand RespondToCommand(Unit unit, UnitCommand command) => null;

	public virtual OpInterruptType TryInterruptOp(Unit unit, UnitOp op) => OpInterruptType.PASS;

	public List<MaterialCache> materialCaches = new List<MaterialCache>();
	public Dictionary<Renderer, Material> rendToMatLookup;

	private void Awake()
	{
		_root = transform.Find("root");
		clipHandler = GetComponent<ClipHandler>();
		hologramMat = Resources.Load<Material>(hologramMatPath);
	}

	bool holographEnabled;

	public EditorButton enableHologramBtn = new EditorButton("EnableHologram", true);
	public void EnableHologram()
	{
		if(hologramMat == null)
		{
			hologramMat = Resources.Load<Material>(hologramMatPath);
			if (hologramMat == null)
				return;
		}

		foreach (var cache in materialCaches)
		{
			if (cache != null && cache.renderer != null)
			{
				cache.renderer.material = hologramMat;
			}
		}
	}

	public EditorButton disableHologramBtn = new EditorButton("DisableHologram", true);
	public void DisableHologram()
	{
		if (hologramMat == null)
		{
			hologramMat = Resources.Load<Material>(hologramMatPath);
			if (hologramMat == null)
				return;
		}

		foreach (var cache in materialCaches)
		{
			if (cache != null && cache.renderer != null)
			{
				cache.renderer.sharedMaterials = cache.materials;
			}
		}
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
	public static void HolsterTo(this Item item, CharacterUnit unit)
	{
		if(unit.holsteredItemLookup.TryGetValue(item.holsteredStyle, out var foundItem))
		{
			Debug.LogWarning(
				$"Already have {foundItem.name} " +
				$"holstered to {item.holsteredStyle.ToString()} " +
				$"on {unit.name}"
				);
			return;
		}

		item.transform.position = unit.transform.position;
		item.transform.rotation = Quaternion.identity;

		if(unit.styleToHolster.TryGetValue(item.holsteredStyle, out var foundBone))
		{
			Debug.LogWarning("holstering to bone pos: " + foundBone.transform.position, foundBone);
			item.root.position = foundBone.transform.position;
			item.root.rotation = foundBone.transform.rotation;
			item.transform.SetParent(foundBone.transform, true);
		}
		 
		if(!unit.inventory.Contains(item))
			unit.inventory.Add(item);
	}

	public static void EquipTo(this Item item, CharacterUnit unit)
	{
		if(unit.equippedItemLookup.TryGetValue(item.equippedStyle, out var foundItem))
		{
			Debug.LogWarning(
				$"Already have {foundItem.name} " +
				$"equipped to {item.equippedStyle.ToString()} " +
				$"on {unit.name}"
				);
			return;
		}
	}

	public static void SetRootFacing(this Item item, Vector3 facingDir)
	{
		item.root.rotation = Quaternion.LookRotation(facingDir);
	}

	public static void SetFacing(this Item item, Vector2Int targetNeighbCoord)
	{
		Vector2Int itemCoord = Board.WorldToOffset(item.root.position);
		var dir = itemCoord.ToNeighbour(targetNeighbCoord);
		item.root.rotation = Quaternion.LookRotation(dir.ToVector());
	}

	//public static void HolsterTo(this Item item, Transform holsterBone)
	//{
	//	item.root.position = holsterBone.position;
	//	item.root.rotation = holsterBone.rotation;
	//	Debug.LogWarning("holstering to bone pos: " + holsterBone.position, holsterBone);
	//	item.transform.SetParent(holsterBone, true);
	//}
}