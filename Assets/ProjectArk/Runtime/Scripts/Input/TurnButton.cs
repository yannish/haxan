using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

[RequireComponent(typeof(TurnButtonFlowController))]
public class TurnButton : RectUIElement
	, IFlowable
{
	//[ReadOnly] public RectUIElement rectElement;
	[ReadOnly] public TurnButtonFlowController turnButtonFlow;

	public FlowController Flow => turnButtonFlow;

	protected override void Awake()
	{
		base.Awake();
		
		turnButtonFlow = GetComponent<TurnButtonFlowController>();

		//TurnButtonFlowController.OnTurnButtonPeeked += (FlowController flow) => Hover();
		//TurnButtonFlowController.OnTurnButtonUnpeeked += (FlowController flow) => Unhover();
		//TurnButtonFlowController.OnTurnButtonEntered += (FlowController flow) => Highlight();
		//TurnButtonFlowController.OnTurnButtonExited += (FlowController flow) => Unhighlight();

		//turnButtonFlow.OnTurnButtonPeeked += (FlowController flow) => Hover();
		//turnButtonFlow.OnTurnButtonUnpeeked += (FlowController flow) => Unhover();
		//turnButtonFlow.OnTurnButtonEntered += (FlowController flow) => Highlight();
		//turnButtonFlow.OnTurnButtonExited += (FlowController flow) => Unhighlight();
	}

	void OnEnable()
	{
		TurnButtonFlowController.OnTurnButtonPeeked += (FlowController flow) => Hover();
		TurnButtonFlowController.OnTurnButtonUnpeeked += (FlowController flow) => Unhover();
		TurnButtonFlowController.OnTurnButtonEntered += (FlowController flow) => Highlight();
		TurnButtonFlowController.OnTurnButtonExited += (FlowController flow) => Unhighlight();
	}

	void OnDisable()
	{
		TurnButtonFlowController.OnTurnButtonPeeked -= (FlowController flow) => Hover();
		TurnButtonFlowController.OnTurnButtonUnpeeked -= (FlowController flow) => Unhover();
		TurnButtonFlowController.OnTurnButtonEntered -= (FlowController flow) => Highlight();
		TurnButtonFlowController.OnTurnButtonExited -= (FlowController flow) => Unhighlight();
	}

	//private void OnFlowPeeked(FlowController obj)
	//{
	//	Hover();
	//}

	//private void OnFlowUnpeeked(FlowController obj)
	//{
	//	Unhover();
	//}


	//   [Header("TURN BUTTON")]
	////   public PlayableDirector director;
	////   public void EndTurn()
	////   {
	////       //director.Play();
	////   }

	////   public EditorButton playBtn = new EditorButton("Play", true);
	////   public void Play()
	////{
	////       director.Play();
	////   }
}
