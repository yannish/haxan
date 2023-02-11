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

    public virtual List<Vector2Int> GetValidMoves(Vector2Int origin, Unit unit) => null;
    // ^^ given unit's position, where can the ability be targeted?

    //List<Vector2Int> 
    public virtual List<Vector2Int> GetAffectedCells(Vector2Int origin, Vector2Int targetCoord) => new List<Vector2Int>();

    public virtual PooledMonoBehaviour PreviewAffectedCell(Vector2Int origin, Vector2Int affectedCoord) => null;

    public virtual Queue<CellObjectCommand> FetchCommandChain(
        Vector2Int targetCoord,
        Unit unit
        )
    {
        Queue<CellObjectCommand> newCommands = new Queue<CellObjectCommand>();
        return newCommands;

        //Turn newTurn = new Turn();
        //newTurn.ability = this;
        //newTurn.instigator = cellObj;
        //return newTurn;

        //return  new Turn()
        //return Turn.CreateInstance(cellObj, this);
    }

    //public virtual void PreviewEffectOnCell

    // ^^ given where the ability is being targeted, which other cells are going to be effected?

    //public virtual void PreviewEffect(Vector2Int origin, List<Vector2Int> coords) { }
}
