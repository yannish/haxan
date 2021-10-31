using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class DummyTurn : InlineScriptableObject
{
    [ReadOnly] public StepPhase phase;
    [ReadOnly] public Character owner;
}

//[Serializable]
public class Turn : InlineScriptableObject
{
    [ReadOnly] public Character owner;

    [ReadOnly] public Ability ability;

    public Queue<CharacterCommand> commands;

    public static Turn CreateInstance(Character owner, Ability ability)
	{
		var newTurn = ScriptableObject.CreateInstance<Turn>();
		newTurn.owner = owner;
		newTurn.ability = ability;
		newTurn.commands = new Queue<CharacterCommand>();
		return newTurn;

	}

	public Turn(Character owner)
	{
		this.owner = owner;
		//this.commands = commands;
	}
}
