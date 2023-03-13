using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISteppable
{
	void OnPlay();
	void OnComplete();

	void Tick(float timeScale = 1f);
	void OnBeginForwardStep();
	void OnBeginBackwardStep();
}

public class TimeStepSequence : MonoBehaviour
{
    public int timeCreated = -1;

    public int duration = 0;

	public int currStep = 0;

	public List<ISteppable> steppables = new List<ISteppable>();

	public List<Component> steppableComponents = new List<Component>();


	private void Awake()
	{
		Init();
	}

	void Init()
	{
		steppables = GetComponentsInChildren<ISteppable>().ToList();
	}

    public void Tick(float timeScale = 1f)
	{
		foreach(var steppable in steppables)
		{
			steppable.Tick(timeScale);
		}
	}

    public virtual void OnBeginForwardStep()
	{
		foreach (var steppable in steppables)
		{
			steppable.OnBeginForwardStep();
		}
	}

	public virtual void OnBeginBackwardStep()
	{
		foreach (var steppable in steppables)
		{
			steppable.OnBeginBackwardStep();
		}
	}

	public virtual void OnCompletedDuration()
	{

	}

	//public virtual void OnBegin
}
