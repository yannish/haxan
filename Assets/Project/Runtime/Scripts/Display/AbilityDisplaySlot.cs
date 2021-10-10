using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplaySlot : MonoBehaviour
{
    public Image abilityIcon;
    public TextMeshProUGUI abilityText;

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
