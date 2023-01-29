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
	public TextMeshProUGUI abilityName;


	public void Init(BoardUI ui, AbilityV2 ability)
	{
		this.ui = ui;
		this.ability = ability;
		this.icon.sprite = ability.icon;
		this.abilityName.SetText(ability.name.ToUpper());
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ui.OnPointerEnteredAbilityButton(ability);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ui.OnPointerExitedAbilityButton(ability);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ui.OnPointerClickedAbilityButton(ability);
	}

}
