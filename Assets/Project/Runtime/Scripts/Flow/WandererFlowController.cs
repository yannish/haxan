using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WandererFlowController : CharacterFlow
{
	[ReadOnly] public Wanderer wanderer;

	protected override void Awake()
	{
		base.Awake();
		wanderer = GetComponent<Wanderer>();
	}

	private void Start()
	{
		wanderer.movementAbility.flow.ProvideCharacter(this);
	}


	//public override void Enter()
	//{
	//	base.Enter();
	//	//Globals.SelectedWanderer.Add(wanderer);
	//}

	//public override void Exit()
	//{
	//	base.Exit();
	//	//Globals.SelectedWanderer.Remove(wanderer);

	//	//if(pathControl != null)
	//	//	pathControl.Discard();
	//}

	//public bool Tick()
	//{
	//	if (Input.GetKeyDown(KeyCode.J))
	//		Debug.Log("keystroke in wanderer flow");

	//	return !inputCommandStack.IsNullOrEmpty();
	//}



	public override void HandleEmptyInput(EmptyClickEvent e)
	{
		if (subFlow == character.movementAbility.flow)
			return;

		base.HandleEmptyInput(e);
	}

	//public override FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	//{
	//	return FlowState.YIELD;
	//}
}
