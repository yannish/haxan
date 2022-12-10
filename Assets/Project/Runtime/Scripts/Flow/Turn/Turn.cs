using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DummyTurn : InlineScriptableObject
{
    [ReadOnly] public StepPhase phase;
    [ReadOnly] public Character owner;
}

[Serializable]
public class Turn// : InlineScriptableObject
{
    [ReadOnly] public CellObject owner;

    [ReadOnly] public Ability ability;

    public Queue<CharacterCommand> commands = new();

    public static Turn CreateInstance(CellObject owner, Ability ability)
	{
		//var newTurn = ScriptableObject.CreateInstance<Turn>();
		var newTurn = new Turn(owner);
		//newTurn.owner = owner;
		newTurn.ability = ability;
		newTurn.commands = new Queue<CharacterCommand>();
		return newTurn;
	}

	public Turn(CellObject owner)
	{
		this.owner = owner;
		//this.commands = commands;
	}
}
