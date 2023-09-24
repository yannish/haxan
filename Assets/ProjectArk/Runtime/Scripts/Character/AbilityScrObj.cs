using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScrObj : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public StepPhase_OLD phase;

    public AbilityType_OLD type;


    public virtual List<Cell_OLD> GetValidMoves(Cell_OLD cell, CharacterFlowController flow) => null;

    public virtual Queue<CellObjectCommand> FetchCommandChain(
        Cell_OLD targetCell,
        CellObject cellObj,
        FlowController flow
        )
    {
        Queue<CellObjectCommand> newCommands = new Queue<CellObjectCommand>();
        return newCommands;
    }

    public virtual Action Peek(Cell_OLD targetCell, CharacterFlowController flow) => null;

    public virtual Action Unpeek() => null;
}
