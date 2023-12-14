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

    [ReadOnly] public AbilityScrObj ability;

    [ReadOnly] public AbilityFlowController abilityFlow;

    public FlowController Flow => abilityFlow != null ? abilityFlow : null;

    //public void DisplayAbility(ScrObjAbility ability)
    //{
    //    gameObject.SetActive(true);

    //    if (ability.icon)
    //        abilityIcon.sprite = ability.icon;

    //    abilityText.SetText(ability.name.ToUpper());
    //}

    //public void Clear()
    //{
    //    gameObject.SetActive(false);
    //}

 //   public void BindTo(Ability ability)
	//{
 //       ability.flow.OnFlowPeeked += OnFlowPeeked;
 //       ability.flow.OnFlowUnpeeked += OnFlowUnpeeked;
        
 //       ability.flow.OnFlowEntered += OnFlowEntered;
 //       ability.flow.OnFlowExited += OnFlowExited;
 //   }

    public void OnFlowPeeked(FlowController obj) => Hover();

    public void OnFlowUnpeeked(FlowController obj) => Unhover();

    public void OnFlowEntered(FlowController obj) => Highlight();

    public void OnFlowExited(FlowController obj) => Unhighlight();
}
