using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityTypeV2
{
    MOVEMENT,
    TARGET,
    PASSIVE
}

//... vague thought with StepPhase is that ITB-style, some things resolve in a set order.
public enum StepPhaseV2
{
    UTILITY,
    MOVE,
    ATTACK
}

public class AbilityV2 : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public StepPhaseV2 phase;

    public AbilityTypeV2 type;


    public virtual void ShowValidMoves(Vector2Int origin, Unit unit) { }

    public virtual void HideValidMoves() { }

    public virtual void ShowPreview(Vector2Int target, Unit unit) { }

    public virtual void HidePreview() { }


    public virtual List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit) => null;
    // ^^ given unit's position, where can the ability be targeted?

    public virtual List<Vector2Int> GetAffectedCells(Vector2Int origin, Vector2Int targetCoord, Unit unit) => new List<Vector2Int>();

    public virtual PooledMonoBehaviour PreviewAffectedCell(Vector2Int origin, Vector2Int affectedCoord) => null;

    public virtual Queue<UnitCommand> FetchCommandChain(Vector2Int targetCoord, Unit unit) => new Queue<UnitCommand>();
}
