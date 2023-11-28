using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial struct UnitOp_STRUCT
{
	public UnitOp_STRUCT GroundMoveOp(
		Unit mover,
		Vector2Int fromCoord,
		Vector2Int toCoord,
		float startTime,
		float duration
		)
	{
		UnitOp_STRUCT newOp = new UnitOp_STRUCT();

		newOp.opType = UnitOpType.GROUND_MOVE;

		newOp.playbackData = new OpPlaybackData(mover, startTime, duration);
		
		newOp.fromCoord = fromCoord;
		newOp.toCoord = toCoord;
		newOp.startPos = fromCoord.ToWorld();
		newOp.endPos = toCoord.ToWorld();

		return newOp;
	}

	public void ExecuteGroundMoveOp(Unit unit)
	{
		unit.DecrementMove();
		unit.SetVisualPos(Vector3.zero, true);
		unit.MoveTo(toCoord);
	}

	public void UndoGroundMoveOp(Unit unit)
	{
		unit.IncrementMove();
		unit.SetVisualPos(Vector3.zero, true);
		unit.MoveTo(fromCoord);
	}

	public void TickGroundMoveOp(Unit unit, float t) => unit.SetVisualPos(Vector3.Lerp(startPos, endPos, t));
}
