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
	public const string path = "path";
	public const string unpath = "unpath";
	public const string hintPath = "hintPath";
	public const string unhintPath = "unhintPath";
}

public enum FSMtrigger
{
	hover,
	unhover,
	select,
	deselect,
	clickable,
	unclickable,
	path,
	unpath,
	hintPath,
	unhintPath
}

public static class FSMExtension { }


public enum FlowState
{
	RUNNING,    //Return when you do something and want to linger in this flow.
	DONE,       //Return when you've done something and want to exit this flow.
	YIELD       //Return when you don't do anything, allowing parent controller to do something.
}


public abstract class FlowController : MonoBehaviour
//public abstract class FlowController<T> : MonoBehaviour where T : MonoBehaviour
{
	public bool logDebug;

	[Header("FLOW:")]
	[ReadOnly] public FlowController subFlow;// { get; set; }
	[ReadOnly] public FlowController peekedFlow;
	[ReadOnly] public FlowController lastSubFlow;

	/*
	 * is it odd to give each flow a Peekd Flow?
	 * There's only ever one of these. 
	 *	one peeked, one selected.
	 */

	//[ReadOnly] 
	//public CellObject baseCellObject;

	

	public event Action<FlowController> OnFlowPeeked = delegate { };
	public event Action<FlowController> OnFlowUnpeeked = delegate { };

	public event Action<FlowController> OnFlowEntered = delegate { };
	public event Action<FlowController> OnFlowExited = delegate { };



	protected virtual void Awake() { }

	public virtual FlowState Tick() { return FlowState.YIELD; }

	public virtual void Enter() => OnFlowEntered(this);

	public virtual void Exit() => OnFlowExited(this);

	public virtual void HoverPeek() => OnFlowPeeked(this);

	public virtual void HoverUnpeek() => OnFlowUnpeeked(this);

	public virtual FlowState HandleInput(ElementClickedEvent e, FlowController parentController = null) { return FlowState.YIELD; }
	//{
	//	if (e.element.flowController == null)
	//		return FlowState.YIELD;

	//	if(subFlow != null)
	//	{
	//		var subFlowState = subFlow.HandleInput(e, parentController);

	//		switch(subFlowState)
	//		{
	//			case FlowState.RUNNING:
	//				return FlowState.RUNNING;

	//			case FlowState.DONE:
	//				TransitionTo(null);
	//				peekedFlow = e.element.flowController;
	//				peekedFlow.HoverPeek();
	//				return FlowState.RUNNING;

	//			case FlowState.YIELD:
	//				break;
	//		}
	//	}

	//	//... clicking the element you're already in:
	//	if (e.element.flowController == this)
	//	{
	//		Debog.logGameflow("clicked existing element");
	//		//TransitionTo(null);
	//		return FlowState.DONE;
	//	}

	//	if(e.element.flowController.GetType() == this.GetType())
	//	{
	//		Debog.logGameflow("clicked element of same type");
	//		return FlowState.YIELD;
	//	}

	//	TransitionTo(e.element.flowController);

	//	return FlowState.RUNNING;
	//}

	public virtual FlowState HandleBackInput(ElementBackClickedEvent e, FlowController parentController = null)
	{
		Debug.LogWarning("handling back input on " + gameObject.name, this.gameObject);

		if (subFlow != null)
		{
			//... you've right clicked the subflow you're currently in, keep it hovered:
			if (e.element.flowController == subFlow)
			{
				TransitionTo(null, false);
				//peekedFlow = e.element.flowController;
				//peekedFlow.HoverPeek();
				return FlowState.YIELD;
			}

			//... you've right clicked somewhere else, but have a subflow, it should probably fold up:
			var result = subFlow.HandleBackInput(e);
			if(result == FlowState.DONE)
			{
				TransitionTo(null, false);
				//if (e.element.flowController != null)
				//	HandleHover(e);
				return FlowState.YIELD;
			}

			//if (subFlow.subFlow == null)
			//{
			//	TransitionTo(null);
			//	return FlowState.YIELD;
			//}
			//else
			//{
			//	//Debug.Log("... subflow had a subflow: " + subFlow.subFlow.gameObject.name);
			//	var result = subFlow.HandleBackInput(e);
			//	if(result == FlowState.DONE)
			//	{
			//		TransitionTo(null);
			//		return FlowState.YIELD;
			//	}
			//}
		}

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

			////Debug.Log("... subflow : " + subFlow.gameObject.name);
			//if (subFlow.subFlow == null)
			//{
			//	//Debug.Log("... subflow doesn't have a subflow" );
			//	TransitionTo(null);
			//	return;
			//}
			//else
			//{
			//	//Debug.Log("... subflow had a subflow: " + subFlow.subFlow.gameObject.name);
			//	subFlow.HandleEmptyInput(e);
			//}
		}
	}

	//... if a flow's HandleHover returns true, it's been "used"...?
	public virtual bool HandleHover(ElementHoveredEvent e)
	{
		if (subFlow != null && subFlow.HandleHover(e))
			return true;

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

		if(logDebug)
			Debog.logInput("Handling hover in " + gameObject.name);

		return false;

		//if (subFlow != null)
		//	return subFlow.HandleHover(e);
		////Debog.logInput("Handling hover in " + gameObject.name);
		//return false;
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

		Debog.logInput("transitioning to " + nextController + " controller");

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
		}
	}
}
