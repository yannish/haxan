using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCommand : CharacterCommand
{
	public HexDirection fromDir;
	public HexDirection toDir;

	private Vector3 currFacing;
	private Vector3 endFacing;


	public TurnCommand(
		CharacterFlow characterFlow, 
		HexDirection fromDir, HexDirection toDir,
		float duration
		) : base(characterFlow)
	{
		this.fromDir = fromDir;
		this.toDir = toDir;
		this.duration = duration;

		this.currFacing = fromDir.ToVector();
		this.endFacing = toDir.ToVector();
	}

	public override void End()
	{
		characterFlow.character.SetFacing(toDir);
	}

	public override bool Tick()
	{
		//currFacing = ;
		characterFlow.character.SetDirectFacing(Vector3.Slerp(currFacing, endFacing, currProgress));

		return base.Tick();
	}
}
