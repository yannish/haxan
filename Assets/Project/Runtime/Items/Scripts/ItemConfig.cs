using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HolsteredStyle
{
    BACK,
    HIP
}

public enum EquippedStyle
{
    HAND,
    HEAVY,
    BACK
}

public enum BoneType
{
    RIGHTHAND,
    LEFTHAND,
    HIPS,
    BACKHOLSTER,
    HIPHOLSTER,
}

public class ItemConfig : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public GameObject propPrefab;

    public HolsteredStyle holsteredStyle;

    public bool requiresReadying = false;

    public List<ItemUseConfig> uses = new List<ItemUseConfig>();


	//.... UI:
	internal void ShowPreview(Vector2Int target, Unit unit) { }

    internal void HidePreview() { }

    public virtual List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit) => null;

    public virtual UnitCommand RespondToCommand(Unit unit, UnitCommand command) => null;

    public virtual void RespondToCommandStep(Unit unit, ref UnitCommandStep commandStep) { }

    public virtual void ShowPathReaction(Vector2Int origin, List<Vector2Int> path) { }

    public virtual void HidePathReaction() { }

    public virtual void ShowAbilityReaction(Vector2Int origin, Unit unit, Ability ability) { }

    public virtual void HideAbilityReaction() { }

    public virtual Queue<UnitCommandStep> FetchCommandStepChain(Vector2Int targetCoord, Unit unit) => new Queue<UnitCommandStep>();


    //... WIELD:
    public virtual void OnEquip(Unit unit) { }

	public virtual void OnUnequip() { }

}

