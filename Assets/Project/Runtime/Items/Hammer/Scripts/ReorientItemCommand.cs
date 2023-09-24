using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReorientItemCommand : UnitCommand
{
	public Item item;

	private Vector3 fromFacing;
	private Vector3 toFacing;

	public ReorientItemCommand(
		Unit unit,
		Item item,
		Vector3 fromFacing,
		Vector3 toFacing,
		float duration
		)
	{
		this.unit = unit;
		this.item = item;
		this.fromFacing = fromFacing;
		this.toFacing = toFacing;
	}

	public override void Execute() => item.SetRootFacing(toFacing);

	public override void Undo() => item.SetRootFacing(fromFacing);

	public override void OnBeginTick()
	{
		//hammer.Set
	}

	public override void OnBeginReverseTick()
	{
		base.OnBeginReverseTick();
	}

	public override bool Tick_OLD(float timeScale = 1)
	{
		base.Tick_OLD(timeScale);
		item.SetRootFacing(Vector3.Slerp(fromFacing, toFacing, currProgress));
		return CheckComplete(timeScale);
	}
}
