using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TODO:
/// 
/// maybe item instances need to run their out little UI universe, like boardUI does.
/// they get their itemSlot and know about all their buttons, so that they can flip icons, and recontextualize based
/// on their current state?
/// 
/// </summary>

[CreateAssetMenu(menuName = "Item Uses/ReadyHammer", fileName = "ReadyHammer")]
public class ReadyHammerUse : ItemUseConfig
{
	public Sprite readyIcon;
	public Sprite unreadyIcon;

	public AnimationClip readyClip;
	public AnimationClip drawClip;

	[Header("VISUALS:")]
	public PooledMonoBehaviour hologramMarker;
	PooledMonoBehaviour hologramInstance;



	public override bool IsReadyUse => true;

	public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit, Item item)
	{
		return origin.GetCardinalRing(1).Where(t => Board.GetUnitAtPos(t) == null).ToList();
	}

	public override void UpdateButton(ItemUseButton useButton, Item item)
	{
		useButton.icon.sprite = item.isReadied ? unreadyIcon : readyIcon;
	}

	public override void ShowPreview(Vector2Int target, Unit unit, Item item) 
	{
		Vector3 unitWorldPos = Board.OffsetToWorld(unit.OffsetPos);
		Vector3 targetWorldPos = Board.OffsetToWorld(target);
		Vector3 unitToTargetDir = unitWorldPos.To(targetWorldPos);

		Hammer hammer = item as Hammer;

		hammer.transform.SetParent(null, true);
		hammer.transform.position = unitWorldPos;
		hammer.transform.rotation = Quaternion.identity;

		hammer.root.localPosition = Vector3.zero;
		hammer.root.rotation = Quaternion.identity;

		hammer.clipHandler.SetMainWeight(1f);
		hammer.EnableHologram();
		hammer.SetRootFacing(unitToTargetDir);
		hammer.SetReadyAnim();
		hammer.TickReadyAnim(1f);
	}

	public override void HidePreview(Vector2Int target, Unit unit, Item item)
	{
		Hammer hammer = item as Hammer;

		hammer.clipHandler.SetMainWeight(0f);
		hammer.DisableHologram();
		hammer.HolsterTo((CharacterUnit)unit);

		//if(hologramInstance != null)
		//	hologramInstance.Stop();
		//hologramInstance = null;
	}

	//public override Queue<UnitCommand> Fetch
}
