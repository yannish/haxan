using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StepPhase
{
    UTILITY,
    MOVE,
    ATTACK
}


public class DummyTurn : InlineScriptableObject
{
    [ReadOnly] public StepPhase phase;
    [ReadOnly] public Character owner;
}

//[Serializable]
public class Turn : InlineScriptableObject
{
    [ReadOnly] public StepPhase phase;

    [ReadOnly] public Character owner;

    public Queue<CharacterCommand> commands;

    //public Turn(Character owner)
    //{
    //    this.owner = owner;
    //    //this.commands = commands;
    //}
}
