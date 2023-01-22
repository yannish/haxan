using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScrObj : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public StepPhase phase;

    public AbilityType type;


    public virtual List<Cell> GetValidMoves(Cell cell, CharacterFlowController flow) => null;

    public virtual Queue<CellObjectCommand> FetchCommandChain(
        Cell targetCell,
        CellObject cellObj,
        FlowController flow
        )
    {
        Queue<CellObjectCommand> newCommands = new Queue<CellObjectCommand>();
        return newCommands;
    }

    public virtual Action Peek(Cell targetCell, CharacterFlowController flow) => null;

    public virtual Action Unpeek() => null;
}
