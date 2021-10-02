using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//... doing this because a cell might give you a flow, or an icon on the HUD (ex. an ability or item)
//public interface IFlowable
//{
//	FlowController Flow { get; }
//}

public class UIElement : MonoBehaviour,
	IPointerEnterHandler,
	IPointerExitHandler,
	IPointerDownHandler,
	IPointerUpHandler
{

	public bool logDebug;


	//... this'll be Cell on the board, yielding the bound -> cellObj -> flowController?
	public IFlowable flowableComponent { get; protected set; }

	public virtual FlowController flowController
	{
		get
		{
			if (flowableComponent != null)
				return flowableComponent.Flow;

			Debog.logInput("return null on element's flowcontroller");
			return null;
		}
	}


	void Awake() =>	flowableComponent = GetComponent<IFlowable>();


	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if(logDebug)
			Debug.Log("poined entered: " + gameObject.name);

		Events.instance.Raise(new ElementHoveredEvent(this));
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (logDebug)
			Debug.Log("poined exited: " + gameObject.name);

		Events.instance.Raise(new ElementHoveredEvent(null));
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		switch(eventData.button)
		{
			case PointerEventData.InputButton.Left:
				if (logDebug)
					Debug.Log("left clicked: " + gameObject.name);
				Events.instance.Raise(new ElementClickedEvent(this));
				break;

			case PointerEventData.InputButton.Right:
				if (logDebug)
					Debug.Log("right clicked: " + gameObject.name);
				Events.instance.Raise(new ElementBackClickedEvent(this));
				break;
		}

		if (eventData.button != PointerEventData.InputButton.Left)
			return;
	}

	public virtual void OnSelect(BaseEventData eventData) { }
	public virtual void OnDeselect(BaseEventData eventData) { }

	public virtual void OnLeft() { }
	public virtual void OnRight() { }

	public virtual void OnPointerUp(PointerEventData eventData) { }
}
