using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public virtual UnitCommand RespondToCommand(Unit unit, UnitCommand command) => null;

    public virtual void RespondToCommandStep(Unit unit, ref UnitCommandStep commandStep) { }

    public virtual void ShowPathReaction(Vector2Int origin, List<Vector2Int> path) { }

    public virtual void HidePathReaction() { }

    public virtual void ShowAbilityReaction(Vector2Int origin, Unit unit, Ability ability) { }

    public virtual void HideAbilityReaction() { }

}
