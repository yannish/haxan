//using Sirenix.OdinInspector;

using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlFlowManager : MonoBehaviour 
{
	public Action EditorUpdater { get; set; }
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	void CallEditorUpdater()
	{
		//EditorUpdater?.Invoke();
		//if (this.EditorUpdater != null)
		//	this.EditorUpdater();
	}

	public CellObject selectedCell; 

	public FlowController initialFlowController;
	public FlowController currFlowController;

	public void ShowCurrentController()
	{
		currFlowController = GetLowestController();
	}

	public FlowController GetLowestController()
	{
		if (initialFlowController == null)
			return null;

		var checkController = initialFlowController;

		while(checkController.subFlow != null)
		{
			checkController = checkController.subFlow;
		}

		return checkController;
	}

	private void Start()
	{
		initialFlowController = GetComponentInChildren<FlowController>();
		if(initialFlowController)
			initialFlowController.Enter();

		Events.instance.AddListener<ElementHoveredEvent>(HandleHover);
		Events.instance.AddListener<ElementBackClickedEvent>(HandleBackClick);
		Events.instance.AddListener<ElementClickedEvent>(HandleInput);
		Events.instance.AddListener<EmptyClickEvent>(HandleEmptyInput);
	}

	private void HandleBackClick(ElementBackClickedEvent e)
	{
		initialFlowController.HandleBackInput(e);
	}

	void HandleHover(ElementHoveredEvent e)
	{
		initialFlowController.HandleHover(e);
	}

	void HandleInput(ElementClickedEvent e)
	{
		initialFlowController.HandleInput(e, null);
	}

	void HandleEmptyInput(EmptyClickEvent e)
	{
		initialFlowController.HandleEmptyInput(e);
	}
}
