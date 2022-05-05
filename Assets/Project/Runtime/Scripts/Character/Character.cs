using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class Character : CellObject
{
    //public static event Action<Character> OnHovered = delegate { };
    //public static event Action OnUnhovered = delegate { };

	public Ability movementAbility;

    
    //... gamestate:
    public bool isStunned;

    public int maxActions;
    [ReadOnly] public int currActions = -1;

    public int maxMoves;
    [ReadOnly] public int currMove = -1;


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
	}
}
