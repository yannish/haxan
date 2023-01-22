using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class Character : CellObject
{
    public event Action<Character> OnCharacterChanged = delegate { };

    //public static event Action<Character> OnHovered = delegate { };

    //public static event Action OnUnhovered = delegate { };

    //public Ability movementAbility;

    public AbilityScrObj movementAbilityScrObj;

    [ReadOnly] public AbilityFlowController movementAbilityFlow;
    
    //... gamestate:
    [Header("STATE: ")]
    public bool isStunned;

    public int maxActions;
    [ReadOnly] public int currActions = -1;

    public int maxMoves;
    [ReadOnly] private int currMove = -1;
    public int CurrMove => currMove;


    public void DecrementMove()
	{
        currMove--;
        OnCharacterChanged?.Invoke(this);
	}

    public void IncrementMove()
    {
        currMove++;
        OnCharacterChanged?.Invoke(this);
    }

    public virtual void Ready()
	{
        //Debog.logGameflow("READYING CHARACTER " + this.gameObject.name, gameObject);

        if (isStunned)
        {
            currActions = 0;
            currMove = 0;
            return;
        }

        currActions = maxActions;
        currMove = maxMoves;

        OnCharacterChanged?.Invoke(this);
	}
}
