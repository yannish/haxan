using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICallbackTest : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.LogWarning("CLICKED: " + this.gameObject.name);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.LogWarning("ENTERED: " + this.gameObject.name);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.LogWarning("EXITED: " + this.gameObject.name);
	}
}
