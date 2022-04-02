using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTriggerBehaviour : QuickStateBehaviour
{
    public FSMtrigger trigger;

	public override void Enter()
	{
		base.Enter();
		fsm.SetTrigger(trigger.ToString());
	}
}
