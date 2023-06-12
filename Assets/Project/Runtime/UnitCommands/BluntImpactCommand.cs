using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluntImpactCommand : UnitCommand
{
    public Vector2Int impactedCoord;
    public Vector2Int originCoord;

    public Vector3 impactPoint;
    public Vector3 impactNormal;

	public const string kBluntImpactPath = "Prefabs/Sequences/BluntImpactSequence";

	public BluntImpactCommand(
		Unit unit, 
		Vector2Int impactedCoord, 
		Vector2Int originCoord, 
		Vector3 impactPoint, 
		Vector3 impactNormal,
		float duration
		)
	{
        this.unit = unit;
        this.impactedCoord = impactedCoord;
        this.originCoord = originCoord;
        this.duration = duration;
        this.impactPoint = impactPoint;
		this.impactNormal = impactNormal;

	}

	public override void Execute()
	{
		
	}

	public override void Undo()
	{
		
	}

	public override bool Tick(float timeScale = 1)
	{
		base.Tick(timeScale);
		return CheckComplete(timeScale);
	}
}
