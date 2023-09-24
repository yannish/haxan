using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyHammerCommand : UnitCommand
{
	public Hammer hammer;

	public Vector2Int startPos;
	public Vector2Int endPos;

	//public Vector3 

	public bool wasReadied;

	public override bool StepsTimeForward() => false;

	public ReadyHammerCommand(
		Unit unit, 
		Hammer hammer,
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

	public override void OnBeginTick()
	{
		//... detach hammer, should no longer follow unit:
		//hammer.transform.SetParent(null);

		//... center to tile in worldspace:
		//hammer.transform.position = unit.transform.position;

		//... figure out facing dir:
		Vector3 unitToEndPos = unit.transform.position.To(Board.OffsetToWorld(endPos));

		//... 
		hammer.SetFacing(endPos);
		//hammer.root.rotation = Quaternion.LookRotation(unitToEndPos, Vector3.up);
		hammer.SetReadyAnim();
		hammer.TickReadyAnim(0f);
	}

	public override bool Tick_OLD(float timeScale = 1)
	{
		base.Tick_OLD(timeScale);

		hammer.TickReadyAnim(currProgress);

		return CheckComplete(timeScale);
	}

	public override void OnBeginReverseTick()
	{
		//hammer.SetFacing
	}

	public override void Execute()
	{
		hammer.isReadied = true;
	}

	public override void Undo()
	{
		hammer.isReadied = wasReadied;
	}
}
