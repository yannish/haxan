using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Turn
{
    [ReadOnly] public Unit instigator;

    [ReadOnly] public Ability ability;
    [ReadOnly] public Item item;
    [ReadOnly] public ItemUseConfig itemUse;

    public List<TimeBlock> timeblocks = new List<TimeBlock>();

    public Queue<UnitCommand> commands = new();

    public Stack<UnitCommand> commandHistory = new();

    public Stack<UnitCommandStep> commandStepHistory = new();
    
    public Stack<UnitCommand> undoneCommands = new();
}
