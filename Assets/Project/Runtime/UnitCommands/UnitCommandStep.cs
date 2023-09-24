using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will wrap up the action (command) of an instigating unit, & then track the consequences 
/// of that action.
/// </summary>
public class UnitCommandStep
{
    public Unit instigator;
    public UnitCommand instigatingCommand;
    public List<UnitCommand> consequences = new List<UnitCommand>();

    public UnitCommandStep(Unit instigator, UnitCommand instigatingCommand)
	{
        this.instigator = instigator;
        this.instigatingCommand = instigatingCommand;
	}

    /// <summary>
    /// returns true when the step is done ticking its instigating command.
    /// </summary>
    /// <returns></returns>
    public bool Tick(float timeScale = 1f)
	{
        var result = instigatingCommand.Tick_OLD(timeScale);
        foreach(var command in consequences)
            command.Tick_OLD();

        //... timing of these consequences isn't linked to the instigating command, 
        //... but they had better be wrapped up smoothly by the time the instigating command 
        //... is finishing.

        return result;
	}

    public virtual void BeginTick() 
    {
        instigatingCommand.OnBeginTick();
        foreach(var command in consequences)
            command.OnBeginTick();
    }

    public virtual void CompleteTick() 
    {
        instigatingCommand.OnCompleteTick();
        foreach (var command in consequences)
            command.OnCompleteTick();
    }

    public virtual void BeginReverseTick() 
    {
        instigatingCommand.OnBeginReverseTick();
        foreach (var command in consequences)
            command.OnBeginReverseTick();
    }

    public virtual void CompleteReverseTick() 
    {
        instigatingCommand.OnCompleteReverseTick();
        foreach (var command in consequences)
            command.OnCompleteReverseTick();
    }

    public virtual void Execute() 
    {
        instigatingCommand.Execute();
        foreach (var command in consequences)
            command.Execute();
    }

    public virtual void Undo() 
    {
        instigatingCommand.Undo();
        foreach (var command in consequences)
            command.Undo();
    }

    public virtual void PropagateConsequences()
	{
        //foreach(var command in consequences)

	}
}
