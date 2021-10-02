using BOG;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExtendedStandaloneInputModule : StandaloneInputModule
{
	public static PointerEventData GetPointerEventData(int pointerId = -1)
	{
		PointerEventData eventData;

		//TODO: There's GetPointerData, and GetLastPointerData
		//		Wonder if it's better to use the latter...?

		_instance.GetPointerData(pointerId, out eventData, true);
		return eventData;
	}

	private static ExtendedStandaloneInputModule _instance;

	protected override void Awake()
	{
		base.Awake();
		_instance = this;
	}

	private void LateUpdate()
	{
		//if (EventSystem.current.IsPointerOverGameObject())
		//	Debug.Log("isPointerOverGameObject = true");
		//else
		//	Debug.Log("isPointerOverGameObject = false");

		if (
			(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) 
			|| (Input.GetMouseButtonDown(1))
			)
		{
			Debog.logInput("emptyClick event");
			Events.instance.Raise(new EmptyClickEvent());
		}
	}
}