using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
	, IPointerEnterHandler
	, IPointerExitHandler
	, IPointerClickHandler
{
	BoardUI ui;
	AbilityV2 ability;

	public Image icon;
	public Image background;
	public TextMeshProUGUI abilityName;

	public Color deselectedColor;
	public Color selectedColor;
	public Color hoveredColor;

	public void Init(BoardUI ui, AbilityV2 ability)
	{
		this.ui = ui;
		this.ability = ability;
		this.icon.sprite = ability.icon;
		this.abilityName.SetText(ability.name.ToUpper());
	}

	bool hovered;
	bool selected;

	public void OnPointerEnter(PointerEventData eventData)
	{
		ui.OnPointerEnteredAbilityButton(ability, this);
		hovered = true;
		UpdateVisual();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ui.OnPointerExitedAbilityButton(ability, this);
		hovered = false;
		UpdateVisual();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ui.OnPointerClickedAbilityButton(ability, this);
	}

	public void Select()
	{
		selected = true;
		UpdateVisual();
	}

	public void Deselect()
	{
		selected = false;
		UpdateVisual();
	}

	public void Hide()
	{
		selected = false;
		hovered = false;
		UpdateVisual();
	}

	void UpdateVisual()
	{
		if(selected)
		{
			background.color = selectedColor;
			return;
		}

		if(hovered)
		{
			background.color = hoveredColor;
			return;
		}

		background.color = deselectedColor;
	}
}
