using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WandererFlowController))]
public class Wanderer : Character
{
	protected override void OnEnable()
	{
		if (Application.isPlaying)
			Globals.ActiveWanderers.Add(this);
	}

    protected override void OnDisable()
	{
		if (Application.isPlaying)
			Globals.ActiveWanderers.Remove(this);
	}
}
