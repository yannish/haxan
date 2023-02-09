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

	public override List<Vector2Int> GetValidMoves(Vector2Int origin, Unit unit)
	{
		return origin.GetCardinalRing(range);
			//.Select(t => Board.TryGetCellAtPos;
	}

	public override List<Vector2Int> GetAffectedCells(Vector2Int origin, Vector2Int targetCoord)
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
