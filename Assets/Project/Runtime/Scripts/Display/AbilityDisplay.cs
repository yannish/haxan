using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDisplay : MonoBehaviour
{
    public AbilityDisplaySlot[] slots;

    private void Awake()
    {
        slots = GetComponentsInChildren<AbilityDisplaySlot>();

        Character.OnHovered += DisplayAbilities;
        Character.OnUnhovered += ClearAbilities;
    }

    private void ClearAbilities()
    {
        throw new NotImplementedException();
    }

    private void DisplayAbilities(Character obj)
    {
        ClearAbilities();

        //for (int i = 0; i < obj.abilities; i++)
        //{

        //}
    }
}
