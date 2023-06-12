using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerReadyCommand : UnitCommand
{
	public Item hammer;

	public Vector2Int startPos;
	public Vector2Int endPos;
	public bool wasReadied;

	public HammerReadyCommand(
		Unit unit, 
		Item hammer,
		Vector2Int startPos, 
		Vector2Int endPos, 
		float duration
		)
	{
		this.unit = unit;
		this.wasReadied = hammer.isReadied;
		this.startPos = hammer.isReadied ? startPos : unit.OffsetPos;
		this.endPos = endPos;
		this.duration = duration;
	}

	public override void Execute()
	{
		hammer.isReadied = true;
	}

	public override void Undo()
	{
		hammer.isReadied = wasReadied;
	}

	public override bool Tick(float timeScale = 1)
	{
		base.Tick(timeScale);

		return CheckComplete(timeScale);
	}

	//public Hammer
}
