using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuffCommand : CellObjectCommand
{
	public ParticleSystem pfx;

	public PuffCommand(
		CellObject cellObject,
		HexDirection fromDir, 
		HexDirection toDir,
		float duration
		) : base(cellObject)
	{
		//this.fromDir = fromDir;
		//this.toDir = toDir;
		//this.duration = duration;

		//this.currFacing = fromDir.ToVector();
		//this.endFacing = toDir.ToVector();
	}

	public override void Execute()
	{

	}

	public override void Undo()
	{

	}

	public override bool Tick(float timeScale = 1)
	{
		pfx.Simulate(Time.deltaTime * timeScale);

		return base.Tick(timeScale);
	}


}
