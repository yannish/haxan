using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitOpType
{
	TURN = 0,
	GROUND_MOVE = 1,
}


public partial struct UnitOp
{
	public UnitOpType opType;

	public OpPlaybackData playbackData;

	//public int unitIndex;
	//public float startTime;
	//public float duration;
	//public float endTime => startTime + duration;

	//... GROUND MOVE:
	public Vector2Int fromCoord;
	public Vector2Int toCoord;

	public Vector3 startPos;
	public Vector3 endPos;

	//... TURN:
	public HexDirectionFT fromDir;
	public HexDirectionFT toDir;

	private Vector3 fromFacing;
	private Vector3 toFacing;

	public void Execute(Unit unit)
	{
		switch (opType)
		{
			case UnitOpType.TURN:
				break;
			case UnitOpType.GROUND_MOVE:
				break;
			default:
				break;
		}
	}

	public void Tick(Unit unit, float t)
	{
		switch (opType)
		{
			case UnitOpType.TURN:
				TickTurnOp(unit, t);
				break;
			case UnitOpType.GROUND_MOVE:
				TickGroundMoveOp(unit, t);
				break;
			default:
				break;
		}
	}
}
