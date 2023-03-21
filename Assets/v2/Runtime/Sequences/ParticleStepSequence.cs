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

public class ParticleStepSequence : MonoBehaviour
	, ISteppable
{
	public StepMode mode = StepMode.PAUSED;

	public ParticleSystem pfx;
	public int steps;
	public int currStep;

	public EditorButton stepForward = new EditorButton("StepForward", true);
	public void StepForward()
	{
		if (mode == StepMode.REWINDING)
			return;

		if (currStep >= steps)
			return;

		pfx.Play();

		currStep++;
		currTargetTime = currStep * stepSize;

		mode = StepMode.PLAYING;
	}

	public EditorButton stepBackward = new EditorButton("StepBackward", true);
	public void StepBackward()
	{
		if (mode == StepMode.PLAYING)
			return;

		if (currStep <= 0)
			return;

		pfx.Play();

		currStep--;
		currTargetTime = currStep * stepSize;

		mode = StepMode.REWINDING;
	}

	[ReadOnly] public float duration;
	[ReadOnly] public float stepSize;
	[ReadOnly] public float currTargetTime;
	[ReadOnly] public float currPfxTime;
	void Awake()
	{
		duration = pfx.main.duration;
		stepSize = duration / steps;
	}


	void Update()
	{
		switch (mode)
		{
			case StepMode.PAUSED:
				break;
			case StepMode.PLAYING:
				TickForward();
				break;
			case StepMode.REWINDING:
				TickBackward();
				break;
			default:
				break;
		}
	}

	private void TickForward()
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

	private void TickBackward()
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

	public void OnBeginBackwardStep()
	{
		
	}

	public void OnBeginForwardStep()
	{
		
	}

	public void Tick(float timeScale = 1)
	{
		
	}

	public void OnPlay()
	{
		
	}
}
