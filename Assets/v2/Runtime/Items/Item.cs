using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public virtual UnitCommand RespondToCommand(Unit unit, UnitCommand command) => null;
}
