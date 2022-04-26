using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

[RequireComponent(typeof(TurnPhaseFlowController))]
public class TurnButton : RectUIElement, IFlowable
{
	//[ReadOnly] public RectUIElement rectElement;
	[ReadOnly] public TurnPhaseFlowController turnPhaseFlow;

	public FlowController Flow => turnPhaseFlow;

	protected override void Awake()
	{
		base.Awake();
		turnPhaseFlow = GetComponent<TurnPhaseFlowController>();
	}

	//public override void OnPointerEnter(PointerEventData eventData)
	//{
	//	base.OnPointerEnter(eventData);
	//	//rectElement.Hover();
	//}

	//public override void OnPointerExit(PointerEventData eventData)
	//{
	//	base.OnPointerExit(eventData);
	//	//rectElement.Unhover();
	//}

	//public override void OnPointerDown(PointerEventData eventData)
	//{
	//	base.OnPointerDown(eventData);
	//	//rectElement.Click();
	//}

	//public override void OnPointerUp(PointerEventData eventData)
	//{
	//	base.OnPointerUp(eventData);
	//	//rectElement.Unclick();
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
