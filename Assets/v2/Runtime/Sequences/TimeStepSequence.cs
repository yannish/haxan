using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISteppable
{
	void OnPlay();
	void OnComplete();
	int Duration();

	void Tick(float timeScale = 1f);

	void StepForward();
	void StepBackward();

	void TickForward();
	void TickBackward();
}

public class TimeStepSequence : MonoBehaviour
{
	[Header("DEBUG:")]
	public bool debug;
	public EditorButton stepForward = new EditorButton("StepForward", true);
	public void StepForward()
	{
		if (mode == StepMode.REWINDING)
			return;

		if (currStep >= steps)
			return;

		mode = StepMode.PLAYING;

		currStep++;
		currTargetTime = currStep * stepSize;

		OnBeginForwardStep();
	}

	public EditorButton stepBackward = new EditorButton("StepBackward", true);
	public void StepBackward()
	{
		if (mode == StepMode.PLAYING)
			return;

		if (currStep <= 0)
			return;

		mode = StepMode.REWINDING;

		currStep--;
		currTargetTime = currStep * stepSize;

		OnBeginForwardStep();
	}


	[Header("STATE:")]
	public StepMode mode = StepMode.PAUSED;
	public Unit instigator;
	public int steps;
	public int currStep = 0;
	public int timeCreated = -1;
	[ReadOnly] public float duration;
	[ReadOnly] public float stepSize;
	[ReadOnly] public float currTargetTime;

	private void Update()
	{
		if (!debug)
			return;

		Tick();	
	}

	public virtual void TickBackward() { }

	public virtual void TickForward() { }

    public void Tick(float timeScale = 1f)
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

	public virtual void OnBeginForwardStep() { }

	public virtual void OnBeginBackwardStep() { }

	public virtual void OnCompletedDuration() { }
}
