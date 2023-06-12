using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReadyItemButton : MonoBehaviour
	, IPointerEnterHandler
	, IPointerExitHandler
	, IPointerClickHandler
{
	BoardUI ui;
	Item item;

	public Image icon;
	public Sprite readyIcon;
	public Sprite unreadyIcon;


	[Header("DEBUG:")]
	public bool debugInput;


	public void Init(BoardUI ui, Item item)
	{
		this.ui = ui;
		this.item = item;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (debugInput)
			Debug.LogWarning("pointer entered ready button:" + item.name, this.gameObject);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (debugInput)
			Debug.LogWarning("pointer exited ready button:" + item.name, this.gameObject);
	}
}
