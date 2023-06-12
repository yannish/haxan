using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StepMode
{
	PAUSED,
	PLAYING,
	REWINDING
}

public class ParticleStepSequence : TimeStepSequence
{
	public ParticleSystem pfx;

	[ReadOnly] public float currPfxTime;

	void OnEnable()
	{
		duration = pfx.main.duration;
		stepSize = duration / steps;
	}

	public override void TickForward()
	{
		currPfxTime = pfx.time;
		float timeToTarget = currTargetTime - currPfxTime;
		float dt = Mathf.Min(timeToTarget, Time.deltaTime);
		pfx.Simulate(dt, true, false, false);

		if(timeToTarget < Time.deltaTime)
		{
			mode = StepMode.PAUSED;
		}
	}

	public override void TickBackward()
	{
		currPfxTime = pfx.time;
		float timeToTarget = currTargetTime - currPfxTime;
		float dt = Mathf.Min(Mathf.Abs(timeToTarget), Time.deltaTime);
		float newPfxTime = currPfxTime - dt;
		pfx.Simulate(newPfxTime, true, true, false);

		if (Mathf.Abs(timeToTarget) < Time.deltaTime)
		{
			mode = StepMode.PAUSED;
		}
	}

	public void OnComplete()
	{
		
	}

	public override void OnBeginBackwardStep()
	{
		pfx.Play();
	}

	public override void OnBeginForwardStep()
	{
		pfx.Play();
	}
}
