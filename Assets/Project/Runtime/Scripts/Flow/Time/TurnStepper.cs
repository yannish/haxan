using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStepper : MonoBehaviour
{
	//[ReadOnly] public Character owner;

	[Header("Timing:")]
	public float easeTime = 1f;
	public float smoothTime = 1f;

	public int steps = 1;

	[ReadOnly] public int currStep = 0;
	[ReadOnly] public float currTime;
	[ReadOnly] public float targetTime;
	[ReadOnly] public float velocity;

	public virtual float duration { get { return 0f; } }
	public virtual float stepSize { get { return 0f; } }

	[ReadOnly] public List<Character> boundActors = new List<Character>();


	protected virtual void Awake() { }

	//public virtual void Tick(float time) { }

	protected virtual void Update()
	{
		currTime = Mathf.SmoothDamp(currTime, targetTime, ref velocity, smoothTime);

		if (markedForDeath && Mathf.Approximately(currTime, 0f))
		{
			Kill();
		}
		else
		{
			Tick();
		}
	}

	protected virtual void Tick() { }
	protected virtual void Kill()
	{
		gameObject.SetActive(false);
	}

	//public void UpdateStep()
	//{

	//}

	void UpdateTargetTime()
	{
		if (currStep <= 0)
			targetTime = 0f;

		if (currStep > 0 && currStep <= steps)
			targetTime = stepSize * currStep;

		if (currStep > steps)
			targetTime = duration;
	}

	public virtual void StepForward()
	{
		if (markedForDeath)
			return;

		currStep++;
		UpdateTargetTime();
	}

	[ReadOnly] public bool markedForDeath;
	public virtual void StepBackward()
	{
		currStep--;
		if (currStep <= 0 && !markedForDeath)
			markedForDeath = true;
		UpdateTargetTime();
	}
}
