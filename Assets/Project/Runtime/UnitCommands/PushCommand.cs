using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushCommand : UnitCommand
{
    public Vector2Int toCoord;
    public Vector2Int fromCoord;

    public Vector3 startPos;
    public Vector3 endPos;

    public PushCommand(Unit unit, Vector2Int fromCoord, Vector2Int toCoord, float duration)
	{
        this.unit = unit;
        this.toCoord = toCoord;
        this.fromCoord = fromCoord;
        this.duration = duration;

        startPos = Board.OffsetToWorld(fromCoord);
        endPos = Board.OffsetToWorld(toCoord);
    }

	public override void Execute()
	{
		unit.DecrementMove();
		unit.SetVisualPos(Vector3.zero, true);
		unit.MoveTo(toCoord);
	}

	public override void Undo()
	{
		unit.IncrementMove();
		unit.SetVisualPos(Vector3.zero, true);
		unit.MoveTo(fromCoord);
	}

	public override bool Tick(float timeScale = 1f)
	{
		base.Tick(timeScale);
		unit.SetVisualPos(Vector3.Lerp(startPos, endPos, currProgress));
		return CheckComplete(timeScale);
	}
}
