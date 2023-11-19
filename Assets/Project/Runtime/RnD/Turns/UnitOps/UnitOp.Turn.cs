using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial struct UnitOp
{
	public static UnitOp TurnOp(
		Unit unit,
		HexDirectionFT fromDir,
		HexDirectionFT toDir,
		float startTime,
		float duration
		)
	{
		UnitOp newOp = new UnitOp();

		newOp.opType = UnitOpType.TURN;

		newOp.playbackData = new OpPlaybackData(unit, startTime, duration);

		newOp.fromDir = fromDir;
		newOp.toDir = toDir;
		newOp.fromFacing = fromDir.ToVector();
		newOp.toFacing = toDir.ToVector();

		return newOp;
	}

	public void ExecuteTurnOp(Unit unit) => unit.SetFacing(toDir);

	public void UndoTurnOp(Unit unit) => unit.SetFacing(fromDir);

	public void TickTurnOp(Unit unit, float t)
	{
		unit.SetDirectFacing(Vector3.Slerp(fromFacing, toFacing, t));
	}
}
