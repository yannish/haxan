//using Sirenix.OdinInspector;

using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlFlowManager : MonoBehaviour 
{
	public static Action<ElementHoveredEvent> OnElementHovered;
	public static Action<ElementBackClickedEvent> OnElementBackClicked;
	public static Action<ElementClickedEvent> OnElementClicked;
	public static Action<EmptyClickEvent> OnEmptyClick;

	[ReadOnly]
	public FlowController initialFlowController;
	[ReadOnly]
	public FlowController currFlowController;


	private void OnEnable()
	{
		OnElementBackClicked += HandleBackClick;
		OnElementClicked += HandleInput;
		OnEmptyClick += HandleEmptyInput;
		OnElementHovered += HandleHover;
	}

	private void OnDisable()
	{
		OnElementBackClicked -= HandleBackClick;
		OnElementClicked -= HandleInput;
		OnEmptyClick -= HandleEmptyInput;
		OnElementHovered -= HandleHover;
	}



	private void Start()
	{
		initialFlowController = GetComponentInChildren<FlowController>();
		if (initialFlowController)
			initialFlowController.Enter();

		//Events.instance.AddListener<ElementHoveredEvent>(HandleHover);
		//Events.instance.AddListener<ElementBackClickedEvent>(HandleBackClick);
		//Events.instance.AddListener<ElementClickedEvent>(HandleInput);
		//Events.instance.AddListener<EmptyClickEvent>(HandleEmptyInput);
	}


	void HandleBackClick(ElementBackClickedEvent e) => initialFlowController.HandleBackInput(e);

	void HandleHover(ElementHoveredEvent e) => initialFlowController.HandleHover(e);

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
