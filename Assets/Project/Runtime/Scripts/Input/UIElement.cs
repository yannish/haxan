using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//... doing this because a cell might give you a flow, or an icon on the HUD (ex. an ability or item), etc.
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

	//public virtual void Awake() =>	flowableComponent = GetComponent<IFlowable>();

	//... this'll be Cell on the board, yielding the bound -> cellObj -> flowController?
	//public IFlowable flowableComponent { get; protected set; }

	//[ReadOnly, SerializeField] private FlowController _flowController;

	public IFlowable flowable;

	public virtual void Awake()
	{
		flowable = GetComponent<IFlowable>();
		//_flowController
	}

	public virtual FlowController flowController => flowable == null ? null : flowable.Flow;
	//{
	//	get
	//	{


	//		if(_flowController == null)
	//		{
	//			IFlowable flowable = GetComponent<IFlowable>();
	//			if(flowable != null)
	//			{
	//				//Debog.logInput("caching a flowable on " + this.name + ", it's " + flowable.Flow.name);
	//				_flowController = flowable.Flow;
	//				return _flowController;
	//			}

	//			Cell cell = GetComponent<Cell>();
	//			if (cell != null)
	//				Debog.logInput("foundd a cell tho");

	//			if(cell is IFlowable)
	//				Debog.logInput("... which is flowable");

	//			//Debog.logInput("... foudn no iflowable on " + this.name);
	//		}

	//		//Debog.logInput("no flowable component on " + this.name + "'s flowcontroller");

	//		return _flowController;
	//	}
	//}

	
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if(logDebug)
			Debug.Log("pointer entered: " + gameObject.name);

		ControlFlowManager.OnElementHovered.Invoke(new ElementHoveredEvent(this));
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (logDebug)
			Debug.Log("pointer exited: " + gameObject.name);

		ControlFlowManager.OnElementHovered.Invoke(new ElementHoveredEvent(null));
	}

	//public virtual void OnPointerUp(PointerEventData eventData)
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		switch(eventData.button)
		{
			case PointerEventData.InputButton.Left:
				if (logDebug)
					Debug.Log("left clicked: " + gameObject.name);
				ControlFlowManager.OnElementClicked.Invoke(new ElementClickedEvent(this));
				break;

			case PointerEventData.InputButton.Right:
				if (logDebug)
					Debug.Log("right clicked: " + gameObject.name);
				ControlFlowManager.OnElementBackClicked.Invoke(new ElementBackClickedEvent(this));
				break;
		}

		if (eventData.button != PointerEventData.InputButton.Left)
			return;
	}

	public virtual void OnSelect(BaseEventData eventData) { }

	public virtual void OnDeselect(BaseEventData eventData) { }

	public virtual void OnLeft() { }
	
	public virtual void OnRight() { }

	public virtual void OnPointerUp(PointerEventData eventData) 
	{
		if (logDebug)
			Debug.Log("click done : " + gameObject.name);
	}
}
