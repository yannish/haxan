using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepCommandV2 : UnitCommand
{
    public Vector2Int toCoord;
    public Vector2Int fromCoord;

    //public Vector3 startPos;
    //public Vector3 endPos;

    public StepCommandV2(Unit unit, Vector2Int fromCoord, Vector2Int toCoord, float duration)
	{
        this.unit = unit;

        this.toCoord = toCoord;
        this.fromCoord = fromCoord;
        this.duration = duration;
	}

	public override void OnBeginTick()
	{

		//base.OnBeginTick();
	}
}
