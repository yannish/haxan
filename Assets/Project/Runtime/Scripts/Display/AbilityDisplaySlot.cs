using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplaySlot : RectUIElement, IFlowable
{
    [Header("ABILITY:")]
    public Image abilityIcon;
    public TextMeshProUGUI abilityText;
    [ReadOnly] public Ability ability;

    public FlowController Flow => ability != null ? ability.flow : null;

    public void DisplayAbility(ScrObjAbility ability)
    {
        gameObject.SetActive(true);

        if (ability.icon)
            abilityIcon.sprite = ability.icon;

        abilityText.SetText(ability.name.ToUpper());
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}
