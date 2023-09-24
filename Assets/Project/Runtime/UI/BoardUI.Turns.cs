using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BoardUI
{
    Unit currInstigator;
    UnitCommand currCommand;
    UnitCommandStep currCommandStep;
    Queue<UnitCommand> commandsQueueToProcess;
    Queue<UnitCommandStep> commandStepsToProcess;

    public Stack<UnitCommand> currCommandHistory;
    public Stack<UnitCommandStep> currCommandStepHistory;
    public Stack<Turn> turnHistory;

    [Header("TURN PROCESSING:")]
    public bool logTurnDebug;
    
    public float currTime = 0f;
    float prevTime = 0f;

    public float currTimeScale = 0f;

    [ReadOnly] public int commandIndex = -1;

    //public float currWorldTime = 0f;
    //float prevWorldTime = 0f;

    //public float currBlockDuration = 0f;


    //... all the commands inputed & to be processed:
    List<UnitCommand> commandsToProcess;
    List<TimeBlock> timeblockHistory = new List<TimeBlock>();
    TimeBlock currTimeBlock;


    //void ProcessInstigatorReactions(TimeBlock timeblock)
    //{
    //    foreach (var item in timeblock.instigator.inventory)
    //    {
    //        var reaction = item.RespondToCommand(
    //            currTimeBlock.instigator,
    //            currTimeBlock.instigatingCommand
    //            );

    //        timeblock.instigatorReactions.Add(reaction);
    //    }
    //}

    void ProcessReactions()
    {
        //var reactions = new List<UnitCommand>();

        
    }

    void ProcessImpacts()
    {

    }

    void ProcessMovement()
    {

    }

    void StartProcessingCommands(List<UnitCommand> commands)
    {
        if (logTurnDebug)
            Debug.LogWarning("Starting to process commands.");

        DeselectUnit();

        mode = Mode.ProcessingCommands;
        playbackState = TurnPlaybackState.PLAYING;

		currTime = 0f;
        prevTime = -1f;
		currTimeScale = 1f;

        currInstigator = selectedUnit;
        commandsToProcess = commands;

        commandIndex = 0;

        timeblockHistory.Clear();

        currTimeBlock = TimeblockActions.GenerateTimeblock(currInstigator, commandsToProcess[commandIndex]);

        //currTimeBlock = new TimeBlock()
        //{
        //    instigator = currInstigator,
        //    instigatingCommand = commandsToProcess[currCommandIndex]
        //};
        
        //currBlockDuration = commands[0].duration;

        //... include all the extra stuff the instigator WILL do (items reacting to a dash, say):

        //... ex. might be a dash which creates 2, 3 or more attacks which then become the chain
        //ProcessInstigatorReactions();

        //... include board reactions to this full list of stuff they will do this step:
        //ProcessReactions();

        //... resulting movement, accounting for conflicts, now included:
        //ProcessMovement();

        //... hits / strikes / bashes / whatever are now included:
        //ProcessImpacts();

        //... now currTimeBlock is ready to be ticked forward like a clip.
    }


    /* what is a "block"? the atomic moves we process.
        ex. Dash from A => B, generating two Slashes.
        - the Dash is the instigating action
        - processed into two dashing slashes, played continuously.
        - these actions are bound by what instigated them, so together they form a block.
     */

    private void HandleCommandProcessing()
    {
        switch (playbackState)
        {
            case TurnPlaybackState.PAUSED:
                break;

            case TurnPlaybackState.PLAYING:
                HandleTurnForward();
                if (currTimeBlock == null)
                {
                    Debug.LogWarning("DONE WITH TURN, BACK TO FLOW");
                    playbackState = TurnPlaybackState.PAUSED;
                    SelectUnit(lastSelectedUnit);
                    currInstigator = null;
                }
                break;

            case TurnPlaybackState.REWINDING:
                HandleTurnBackward();
                break;

            default:
                break;
        }
    }

    private void HandleTurnForward()
    {
        //... this was... to see if anything was still "busy" while the block was running?
        //... don't think this works. 
        //... smoothly pushing time forward probably means top-down running what's happening, 
        //... not letting stuff within blocks be the judge.
        //bool blockComplete = true;

        currTime += Time.deltaTime * currTimeScale;

        //if (!currTimeBlock.instigatingCommmandOverwritten)
        //    currTimeBlock.instigatingCommand.Tick(currTime, currTimeScale);

        foreach (var command in currTimeBlock.commands)
        {
            if (currTime > command.startTime && prevTime < command.startTime)
                command.OnBeginTick();

			if (currTime < command.endTime)
                command.Tick(currTime, currTimeScale);

            if (currTime > command.endTime && prevTime < command.endTime)
                command.OnCompleteTick();
        }

        prevTime = currTime;

        //foreach (var command in currTimeBlock.boardReactions)
        //{
        //    if (!command.Tick_OLD(currTimeScale))
        //        blockComplete = false;
        //    else
        //    {
        //        command.Execute();
        //    }
        //}

        if (currTime > currTimeBlock.length)
        {
            Board.currTimeStep++;

            if (commandIndex < (commandsToProcess.Count - 1))
            {
                commandIndex++;
                timeblockHistory.Add(currTimeBlock);
                currTimeBlock = TimeblockActions.GenerateTimeblock(currInstigator, commandsToProcess[commandIndex]);
            }
			else
			{
                Debug.LogWarning("All instigating commands have been processed.");

                SelectUnit(currInstigator);
                playbackState = TurnPlaybackState.PAUSED;
                currTimeBlock = null;
			}

            //TODO: what about stuff that hasn't incurred a time cost...
            //... rotations?
            //... equip / unequip?
            //... item use?

            //if (currTimeBlock.instigatingCommand.StepsTimeForward())
            //{
            //}
        }
    }

    void StartProcessingCommands_OLD(Queue<UnitCommand> commands)
    {
        currInstigator = selectedUnit;
        DeselectUnit();

        commandsQueueToProcess = commands;
        //currCommand = commandsToProcess.Dequeue();
        currCommandStep = new UnitCommandStep(currInstigator, commandsQueueToProcess.Dequeue());

        currCommandStep.instigatingCommand.OnBeginTick();
        //currCommand.OnBeginTick();

        //Board.RespondToCommandBeginTick(currInstigator, currCommand);

        currCommandHistory = new Stack<UnitCommand>();
        currCommandStepHistory = new Stack<UnitCommandStep>();

        mode = Mode.ProcessingCommands;
        playbackState = TurnPlaybackState.PLAYING;

        Debug.Log("STARTED PROCESSING COMMANDS");
    }

}

public static class TimeblockActions
{
    ////... ex. might be a dash which creates 2, 3 or more attacks which then become the chain
    //ProcessInstigatorReactions();
    ////... include board reactions to this full list of stuff they will do this step:
    //ProcessReactions();
    ////... resulting movement, accounting for conflicts, now included:
    //ProcessMovement();
    ////... hits / strikes / bashes / whatever are now included:
    //ProcessImpacts();

    public static TimeBlock GenerateTimeblock(Unit instigator, UnitCommand instigatingCommand)
	{
        var newTimeBlock = new TimeBlock
        {
            instigator = instigator,
            instigatingCommand = instigatingCommand
        };

        ProcessInstigatorReactions(ref newTimeBlock);
        ProcessReactions(ref newTimeBlock);
        ProcessMovement(ref newTimeBlock);
        ProcessImpacts(ref newTimeBlock);

        var talliedLength = 0f;
        foreach(var command in newTimeBlock.commands)
		{
            talliedLength += command.duration;
		}

        newTimeBlock.length = talliedLength;

        return newTimeBlock;
    }

    /// <thoughts>
    /// 
    /// This won't look like just a series of additive extra commands going into another list.
    /// Only ONE thing can have control over the unit's actual movement?
    /// 
    /// Will need to prioritize what gets the reaction, then that item or ability fully owns & rebuilds the
    /// Unit's commands for that block / series of blocks.
    /// 
    /// Could have maybe a few categories, one thing actively generating new commands (bashes, slashes, etc.)
    /// And one thing flavoring it with additive visuals (invulnerable, speedy, slippery) that are still 
    /// mechanically important but 
    /// 
    /// </thoughts>
    /// <param name="timeBlock"></param>
    public static void ProcessInstigatorReactions(ref TimeBlock timeBlock)
	{
        foreach (var unit in Board.Units)
        {
            foreach (var item in unit.inventory)
            {
                var reaction = item.RespondToCommand(
                    timeBlock.instigator,
                    timeBlock.instigatingCommand
                    );
                timeBlock.boardReactions.Add(reaction);

                /*
                 * These reactions can have offsets in time.
                 */
            }
        }
    }

    public static void ProcessReactions(ref TimeBlock timeblock)
    {

    }

    public static void ProcessMovement(ref TimeBlock timeblock)
    {

    }

    public static void ProcessImpacts(ref TimeBlock timeblock)
    {

    }
}
