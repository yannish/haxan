//using Sirenix.OdinInspector;

using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlFlowManager : MonoBehaviour 
{
	public static Action<ElementHoveredEvent> OnElementHoverStart;

	public static Action<ElementHoveredEvent> OnElementHoverStop;
	
	public static Action<ElementBackClickedEvent> OnElementBackClicked;
	
	public static Action<ElementClickedEvent> OnElementClicked;
	
	public static Action<EmptyClickEvent> OnEmptyClick;


	[ReadOnly] public FlatFlowController flatFlow;

	[ReadOnly] public FlowController initialFlowController;
	
	[ReadOnly] public FlowController currFlowController;


	private void OnEnable()
	{
		OnElementHoverStart += HandleHoverStartFlat;
		OnElementHoverStop += HandleHoverStopFlat;

		//OnElementBackClicked += HandleBackClick;
		//OnElementClicked += HandleInput;
		//OnEmptyClick += HandleEmptyInput;
		//OnElementHoverStart += HandleHoverStart;
		//OnElementHoverStop += HandleHoverStop;
	}

	private void HandleHoverStopFlat(ElementHoveredEvent e) => flatFlow.HandleHoverStart(e);

	private void HandleHoverStartFlat(ElementHoveredEvent e) => flatFlow.HandleHoverStop(e);

	private void OnDisable()
	{
		OnElementHoverStart -= HandleHoverStartFlat;
		OnElementHoverStop -= HandleHoverStopFlat;

		//OnElementBackClicked -= HandleBackClick;
		//OnElementClicked -= HandleInput;
		//OnEmptyClick -= HandleEmptyInput;
		//OnElementHoverStart -= HandleHoverStart;
		//OnElementHoverStop -= HandleHoverStop;
	}


	private void Start()
	{
		initialFlowController = GetComponentInChildren<FlowController>();
		if (initialFlowController)
			initialFlowController.Enter();
	}

	void HandleBackClick(ElementBackClickedEvent e) => initialFlowController.HandleBackInput(e);

	void HandleHoverStart(ElementHoveredEvent e) => initialFlowController.HandleHoverStart(e);

	void HandleHoverStop(ElementHoveredEvent e) => initialFlowController.HandleHoverStop(e);

	void HandleInput(ElementClickedEvent e) => initialFlowController.HandleInput(e, null);

	void HandleEmptyInput(EmptyClickEvent e) => initialFlowController.HandleEmptyInput(e);


	//public Action EditorUpdater { get; set; }
	//[System.Diagnostics.Conditional("UNITY_EDITOR")]
	//void CallEditorUpdater()
	//{
	//	//EditorUpdater?.Invoke();
	//	//if (this.EditorUpdater != null)
	//	//	this.EditorUpdater();
	//}

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
}
