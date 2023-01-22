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
public class Turn //: ScriptableObject
{
	public float numba;

    [ReadOnly] public CellObject instigator;

    [ReadOnly] public Ability ability;

    public Queue<CellObjectCommand> commands = new();

	public Stack<CellObjectCommand> commandHistory = new();

	public Stack<CellObjectCommand> undoneCommands = new();

 //   public static Turn CreateInstance(CellObject owner, Ability ability)
	//{
	//	//var newTurn = ScriptableObject.CreateInstance<Turn>();
	//	var newTurn = new Turn()
	//	{
	//		owner = owner,
	//		ability = ability,
	//		commands = new Queue<CharacterCommand>(),
	//	};

	//	newTurn.owner = owner;
	//	newTurn.ability = ability;
	//	newTurn.commands = new Queue<CharacterCommand>();
	//	return newTurn;
	//}

	//public Turn()
	//{
	//	//this.owner = owner;
	//	//this.commands = commands;
	//}
}

[Serializable]
public class RecordedTurn
{
	[ReadOnly] public CellObject owner;
	[ReadOnly] public Ability ability;
	[ReadOnly] public Stack<CellObjectCommand> commandHistory = new Stack<CellObjectCommand>();
}