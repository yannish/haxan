using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUseButton : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
{
    public Image icon;

    BoardUI ui;
	[ReadOnly] public Item item;
	[ReadOnly] public ItemUseConfig use;// { get; private set; }

	public Color availableTint;
	public Color unavailableTint;

	public void Init(BoardUI ui, Item item, ItemUseConfig use)
	{
        this.ui = ui;
        this.item = item;
		this.use = use;
		//... TODO: make this overridable for ready-buttons, they have 2 icons:
		this.icon.sprite = use.icon;
		Debug.LogWarning("setting icon: " + use.icon.name);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ui.OnPointerClickItemUseButton(item, this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ui.OnPointerEnterItemUseButton(item, this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ui.OnPointerExitItemUseButton(item, this);
	}

	public void SetUnavailable()
	{
		icon.color = unavailableTint;
	}

	public void SetAvailable()
	{
		icon.color = availableTint;
	}
}
