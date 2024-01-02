using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitiesV2/Bash", fileName = "Bash")]
public class Bash : Ability
{
	public int range = 1;

	
	[Header("TIMING:")]
	public float stepDuration;
	public float turnDuration;

	[Header("VISUALS:")]
	public PooledMonoBehaviour impactParticle;
	public PooledMonoBehaviour pushableMarker;
	public PooledMonoBehaviour unpushableMarker;

	PooledMonoBehaviour pushableMarkerInstance;
	PooledMonoBehaviour unpushableMarkerInstance;


	public override Action ReactToOps(List<UnitOp> ops, Unit instigator, Unit owner)
	{
		if (instigator != owner)
			return null;

		Debug.LogWarning("BASH REACTING TO SOME OPS");
		Action shutDownUI = () => { };
		
		foreach(var op in ops)
		{
			GroundMoveOp groundMoveOp = op as GroundMoveOp;
			if (groundMoveOp == null)
				continue;

			var startingPos = groundMoveOp.fromCoord;

			List<Unit> bashedUnits = new List<Unit>();
			var startingNeighbours = Board.GetNeighbouringUnits(startingPos);
			var endingNeighbours = Board.GetNeighbouringUnits(groundMoveOp.toCoord);
			foreach(var startingNeighbour in startingNeighbours)
			{
				if (endingNeighbours.Contains(startingNeighbour))
					bashedUnits.Add(startingNeighbour);
			}

			foreach(var bashedUnit in bashedUnits)
			{
				var basherUnitWorldPos = startingPos.ToWorld();
				var bashedUnitWorldPos = bashedUnit.OffsetPos.ToWorld();
				var pushableMarkerInstance = pushableMarker.GetAndPlay(bashedUnitWorldPos, basherUnitWorldPos.To(bashedUnitWorldPos));
				shutDownUI += () => pushableMarkerInstance.Stop();
			}
		}

		return shutDownUI;
	}

	public override void ShowPreview(Vector2Int target, Unit unit)
	{
		Vector3 originWorldPos = Board.OffsetToWorld(unit.OffsetPos);
		Vector3 affectedWorldPos = Board.OffsetToWorld(target);
		Vector3 originToAffectedDir = originWorldPos.To(affectedWorldPos);

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
}
