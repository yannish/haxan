using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitiesV2/Bash", fileName = "Bash")]
public class BashV2 : AbilityV2
{
	public int range= 1;

	public PooledMonoBehaviour pushableMarker;
	public PooledMonoBehaviour unpushableMarker;

	//public PooledMonoBehaviour previewMarker;


	public override void ShowValidMoves(Vector2Int origin, Unit unit)
	{
		base.ShowValidMoves(origin, unit);
	}

	public override void HideValidMoves()
	{
		base.HideValidMoves();
	}

	PooledMonoBehaviour pushableMarkerInstance;
	PooledMonoBehaviour unpushableMarkerInstance;

	public override void ShowPreview(Vector2Int target, Unit unit)
	{
		Vector3 originWorldPos = Board.OffsetToWorld(unit.OffsetPos);
		Vector3 affectedWorldPos = Board.OffsetToWorld(target);
		Vector3 originToAffectedDir = originWorldPos.To(affectedWorldPos);

		//var newPreviewMarker;

		if (target.TryGetUnitAtCoord(out var foundUnit))
		{
			if (
				foundUnit.preset != null
				&& foundUnit.preset.knockResistance != KnockResistance.IMMOVABLE
				)
			{
				if(pushableMarkerInstance != null)
				{
					pushableMarkerInstance.Play();
				}
				else
				{
					pushableMarkerInstance = pushableMarker.GetAndPlay(affectedWorldPos, originToAffectedDir);
					pushableMarkerInstance.OnReturnedToPool += () =>
					{
						pushableMarkerInstance = null;
					};
				}
			}
			else
			{
				if (unpushableMarkerInstance != null)
				{
					unpushableMarkerInstance.Play();
				}
				else
				{
					unpushableMarkerInstance = unpushableMarker.GetAndPlay(affectedWorldPos, originToAffectedDir);
					unpushableMarkerInstance.OnReturnedToPool += () =>
					{
						unpushableMarkerInstance = null;
					};
				}
			}
		}
	}

	public override void HidePreview()
	{
		if (unpushableMarkerInstance != null)
			unpushableMarkerInstance.Stop();

		if (pushableMarkerInstance != null)
			pushableMarkerInstance.Stop();
	}

	public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit)
	{
		return origin.GetCardinalRing(range);
	}

	public override List<Vector2Int> GetAffectedCells(Vector2Int origin, Vector2Int targetCoord, Unit unit)
	{
		return new List<Vector2Int> { targetCoord };
	}

	public override PooledMonoBehaviour PreviewAffectedCell(Vector2Int origin, Vector2Int affectedCoord)
	{
		Vector3 originWorldPos = Board.OffsetToWorld(origin);
		Vector3 affectedWorldPos = Board.OffsetToWorld(affectedCoord);
		Vector3 originToAffectedDir = originWorldPos.To(affectedWorldPos);

		PooledMonoBehaviour newMarker = null;

		//newMarker = pushableMarker.GetAndPlay(
		//	affectedWorldPos,
		//	originToAffectedDir
		//	);

		//PooledMonoBehaviour newMarker = null;
		if (affectedCoord.TryGetUnitAtCoord(out var foundUnit))
		{
			if (
				foundUnit.preset != null
				&& foundUnit.preset.knockResistance != KnockResistance.IMMOVABLE
				)
			{
				newMarker = pushableMarker.GetAndPlay(
					affectedWorldPos,
					originToAffectedDir
					);
			}
			else
			{
				newMarker = unpushableMarker.GetAndPlay(
					affectedWorldPos,
					originToAffectedDir
					);
			}
		}

		return newMarker;
	}

	//public override void ShowPreviewEffect(Vector2Int originList, <Vector2Int> coords)
	//{
	//	//Action undo = null;
	//	//return undo;
	//}

	//public override void HidePreviewEffect()
	//{

	//}
}
