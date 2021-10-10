using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : CellObject
{
    public static event Action<Character> OnHovered = delegate { };
    public static event Action OnUnhovered = delegate { };

    //public string characterName;

	public Ability movementAbility;
	public List<Ability> abilities;

    //... gamestate:
    public bool isStunned;
    public int maxMove;
}
