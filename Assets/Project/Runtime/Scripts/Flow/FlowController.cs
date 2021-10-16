using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FSM
{
	public const string hover = "hover";
	public const string unhover = "unhover";
	public const string select = "select";
	public const string deselect = "deselect";
	public const string clickable = "clickable";
	public const string unclickable = "unclickable";
	public const string path = "path";
	public const string unpath = "unpath";
}

public enum FlowState
{
	RUNNING,    //Return when you do something and want to linger in this flow.
	DONE,       //Return when you've done something and want to exit this flow.
	YIELD       //Return when you don't do anything, allowing parent controller to do something.
}

public abstract class FlowController : MonoBehaviour
{
	public bool logDebug;

	[ReadOnly] public FlowController subFlow;// { get; set; }

	[ReadOnly] public FlowController peekedFlow;

	/*
	 * is it odd to give each flow a Peekd Flow?
	 * There's only ever one of these. 
	 *	one peeked, one selected.
	 */

	//[ReadOnly] 
	//public CellObject baseCellObject;

	protected virtual void Awake() { }

	public virtual void Update() { }

	public virtual void Enter() { }

	public virtual void Exit() { }

	public virtual void HoverPeek() { }

	public virtual void HoverUnpeek() { }

	public virtual FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null)
	{
		return FlowState.YIELD;
	}

	public virtual FlowState HandleBackInput(ElementBackClickedEvent e, FlowController parentController = null)
	{
		return FlowState.YIELD;
	}

	public virtual void HandleEmptyInput(EmptyClickEvent e)
	{
		Debug.Log("handling empty input on " + gameObject.name);

		if (subFlow != null)
		{
			//Debug.Log("... subflow : " + subFlow.gameObject.name);
			if (subFlow.subFlow == null)
			{
				//Debug.Log("... subflow doesn't have a subflow" );
				TransitionTo(null);
				return;
			}
			else
			{
				//Debug.Log("... subflow had a subflow: " + subFlow.subFlow.gameObject.name);
				subFlow.HandleEmptyInput(e);
			}
		}
	}

	//... if a flow's HandleHover returns true, it's been "used"...?
	public virtual bool HandleHover(ElementHoveredEvent e)
	{
		if (subFlow != null)
			subFlow.HandleHover(e);
		//Debog.logInput("Handling hover in " + gameObject.name);
		return false;
	}

	protected virtual void TransitionTo(FlowController newFlowController)
	{
		if (peekedFlow != null && peekedFlow != newFlowController)
		{
			//if (peekedFlow != newFlowController)
			peekedFlow.HoverUnpeek();
			peekedFlow = null;
		}

		var nextController = "null";

		if (newFlowController != null)
			nextController = newFlowController.gameObject.name;

		Debog.logInput("transitioning to " + nextController + " controller");

		if (subFlow != null)
		{
			subFlow.Exit();
			subFlow = null;
		}

		subFlow = newFlowController;

		if (subFlow != null)
		{
			//PassControl();
			subFlow.Enter();
		}
	}
}
