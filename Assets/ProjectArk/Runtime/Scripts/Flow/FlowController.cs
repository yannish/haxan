using BOG;
using System;
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

	public const string peekClickable = "peekClickable";
	public const string unpeekClickable = "unpeekClickable";

	public const string path = "path";
	public const string unpath = "unpath";

	public const string peekPath = "peekPath";
	public const string unpeekPath = "unpeekPath";
}

public static class FSMExtension { }

public enum FlowState
{
	RUNNING,    //Return when you do something and want to linger in this flow.
	DONE,       //Return when you've done something and want to exit this flow.
	YIELD       //Return when you don't do anything, allowing parent controller to do something.
}


public abstract class FlowController : MonoBehaviour
{
	public bool logDebug;

	//[Header("FLOW:")]
	[ReadOnly] public FlowController subFlow;
	[ReadOnly] public FlowController peekedFlow;
	[ReadOnly] public FlowController lastSubFlow;

	/*
	 * is it odd to give each flow a Peekd Flow?
	 * There's only ever one of these. 
	 *	one peeked, one selected.
	 */

	public static event Action<FlowController> OnFlowPeeked = delegate { };

	public static event Action<FlowController> OnFlowUnpeeked = delegate { };

	public static event Action<FlowController> OnFlowEntered = delegate { };
	
	public static event Action<FlowController> OnFlowExited = delegate { };


	protected virtual void Awake() { }

	public virtual FlowState Tick() { return FlowState.YIELD; }

	public virtual bool IsEnterable => true;

	public virtual void Enter() => OnFlowEntered(this);

	public virtual void Exit() => OnFlowExited(this);

	public virtual void HoverPeek() => OnFlowPeeked(this);

	public virtual void HoverUnpeek() => OnFlowUnpeeked(this);

	public virtual FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null) 
	{ 
		return FlowState.YIELD; 
	}

	public virtual FlowState HandleBackInput(ElementBackClickedEvent e, FlowController parentController = null)
	{
		return FlowState.RUNNING;
	}

	public virtual void HandleEmptyInput(EmptyClickEvent e)
	{
		if(logDebug)
			Debog.logInput("handling empty input on " + gameObject.name, this.gameObject);

		if (subFlow != null)
		{
			TransitionTo(null);
			return;
		}
	}

	//... if a flow's HandleHover returns true, it's been "used"...?
	public virtual bool HandleHoverStart(ElementHoveredEvent e)
	{
		//if (subFlow != null && subFlow.HandleHover(e))
		//	return true;

		if (peekedFlow != null)
		{
			peekedFlow.HoverUnpeek();
			peekedFlow = null;
		}

		//... 
		if (e.element == null || e.element.flowController == null)
			return false;

		peekedFlow = e.element.flowController;
		peekedFlow.HoverPeek();

		if (logDebug)
			Debog.logInput("Handling hover in " + gameObject.name);

		return false;

		//if (subFlow != null)
		//	return subFlow.HandleHover(e);
		////Debog.logInput("Handling hover in " + gameObject.name);
		//return false;
	}

	public virtual bool HandleHoverStop(ElementHoveredEvent e)
	{
		//if (peekedFlow != null)
		//{
		//	peekedFlow.HoverUnpeek();
		//	peekedFlow = null;
		//}

		return false;
	}

	protected virtual void TransitionTo(FlowController newFlowController, bool clearPeekedFlow = true)
	{
		if (clearPeekedFlow && peekedFlow != null && peekedFlow != newFlowController)
		{
			//if (peekedFlow != newFlowController)
			peekedFlow.HoverUnpeek();
			peekedFlow = null;
		}

		var nextController = "null";

		if (newFlowController != null)
			nextController = newFlowController.gameObject.name;

		Debog.logGameflow("transitioning to " + nextController + " controller");

		if (subFlow != null)
		{
			subFlow.Exit();
			subFlow = null;
		}

		subFlow = newFlowController;
		if(subFlow != null)
			lastSubFlow = subFlow;

		if (subFlow != null)
		{
			//PassControl();
			subFlow.Enter();
			//OnFlowEnteredGlobal(subFlow);
		}
	}
}
