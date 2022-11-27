using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

[RequireComponent(typeof(TurnButtonFlowController))]
public class TurnButton : RectUIElement, IFlowable
{
	//[ReadOnly] public RectUIElement rectElement;
	[ReadOnly] public TurnButtonFlowController turnButtonFlow;

	public FlowController Flow => turnButtonFlow;

	protected override void Awake()
	{
		base.Awake();
		
		turnButtonFlow = GetComponent<TurnButtonFlowController>();

		//turnButtonFlow.OnFlowPeeked += OnFlowPeeked;
		//turnButtonFlow.OnFlowUnpeeked += OnFlowUnpeeked;
		//turnButtonFlow.OnFlowEntered += ClickAndRelease;
	}

	private void OnFlowPeeked(FlowController obj)
	{
		Hover();
	}

	private void OnFlowUnpeeked(FlowController obj)
	{
		Unhover();
	}


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
