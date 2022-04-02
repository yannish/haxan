using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(WandererFlowController))]
public class Wanderer : Character
{
	protected override void OnEnable()
	{
		Globals.ActiveWanderers?.Add(this);
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		Globals.ActiveWanderers?.Remove(this);
		base.OnDisable();
	}

	protected override void Start()
	{
		base.Start();
	}

	private void Awake()
	{
		movementAbility = Instantiate<Ability>(movementAbility, this.transform);
	}
}
