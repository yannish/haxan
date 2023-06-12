using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TurnV2
{
    [ReadOnly] public Unit instigator;

    [ReadOnly] public Ability ability;

    public Queue<UnitCommand> commands = new();

    public Stack<UnitCommand> commandHistory = new();

    public Stack<UnitCommandStep> commandStepHistory = new();
    
    public Stack<UnitCommand> undoneCommands = new();
}
