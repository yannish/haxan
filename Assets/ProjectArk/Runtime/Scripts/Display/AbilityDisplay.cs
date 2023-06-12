using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDisplay : MonoBehaviour
{
    public AbilityDisplaySlot[] slots;

    public int numberOfAbilitySlots;

    public GameObject abilitySlotPrefab;


    private void Start()
    {
		//for (int i = 0; i < numberOfAbilitySlots; i++)
		//{
  //          var newSlot = Instantiate(abilitySlotPrefab);
  //          newSlot.transform.SetParent(this.transform);
  //      }

  //      foreach (var wanderer in Globals.ActiveWanderers.Items)
		//{

		//}

        slots = GetComponentsInChildren<AbilityDisplaySlot>();

		//CellObjFlowController.Peeked += DisplayAbilities;
  //      CellObjFlowController.Unpeeked += ClearAbilities;

        CellObjFlowController.OnEntered += DisplayAbilities;
        CellObjFlowController.OnExited += ClearAbilities;

        ClearAbilities();
	}

    private void ClearAbilities(CellObjFlowController obj = null)
    {
		foreach (var slot in slots)
		{
            if (slot == null)
                continue;

            slot.gameObject.SetActive(false);
		}
    }

    private void DisplayAbilities(CellObjFlowController obj)
    {
        ClearAbilities();

        //foreach (var slot in slots)
        //{
        //    if (slot == null)
        //        continue;

        //    slot.gameObject.SetActive(true);
        //}

        //foreach (var ability in obj.cellObject.abilities)
        //{
        //    if (ability == null)
        //        continue;

        //    AbilityDisplaySlot slot = slots[abilityCount];
        //    if (slot == null)
        //        continue;

        //    SlotAbility(ability, slot);

        //    abilityCount++;
        //}


        int abilityCount = 0;
        foreach(var flow in obj.cellObject.abilityFlows)
		{
            if (flow == null)
                continue;

            AbilityDisplaySlot slot = slots[abilityCount];
            if (slot == null)
                continue;

            SlotAbility(flow, slot);

            abilityCount++;
		}
    }

	private void SlotAbility(AbilityFlowController abilityFlow, AbilityDisplaySlot slot)
	{
        slot.gameObject.SetActive(true);
        slot.abilityIcon.sprite = abilityFlow.abilityScrObj.icon; 
        slot.abilityText.SetText(abilityFlow.abilityScrObj.name.ToUpper());
        slot.ability = abilityFlow.abilityScrObj;
        slot.abilityFlow = abilityFlow;

        //ability.flow.OnFlowPeeked += slot.OnFlowPeeked;
        //ability.flow.OnFlowUnpeeked += slot.OnFlowUnpeeked;

        //ability.flow.OnFlowEntered += slot.OnFlowEntered;
        //ability.flow.OnFlowExited += slot.OnFlowExited;
	}
}
