using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the highest-level chunk, handled by Board.
/// In between the stop & start of a TimeBlock we run Processing steps arbitrarily. 
/// Maybe to be exposed to player, or left to be sussed out.
/// 
/// </summary>
/// 
[Serializable]
public class TimeBlock
{
    //... just for ease of access, the unit who's turn is yielding this:
    public Unit instigator;
    
    //... and the original action they took:
    public UnitCommand instigatingCommand;

    //... the actual processed commmands that will run:
    public List<UnitCommand> commands = new List<UnitCommand>();

    public float length;

    //... if reactions replace the instigating command:
    //public bool instigatingCommmandOverwritten;

    //... ALL of the commands taking place. Includes other reactions.
    public List<UnitCommand> boardReactions = new List<UnitCommand>();

    
    /// <thoughts>
    /// Tick returning true / false leaves Commands to decide when things are done.
    /// This seems like it needs to change.
    /// Commands should be fit into a set window of time.
    /// </thoughts>


    //... maybe this logic just belongs with TurnProcessing. just make this data.
 //   public bool Tick()
	//{
 //       bool done = true;

 //       if(!instigatingCommmandOverwritten)
	//	{
 //           if(!instigatingCommand.Tick())
 //               done = false;
	//	}
	//	else
	//	{
 //           foreach(var command in instigatorReactions)
	//	    {
 //               if(!command.Tick())
 //                   done = false;
	//	    }
	//	}

 //       foreach(var command in boardReactions)
	//	{
 //           if (!command.Tick())
 //               done = false;
	//	}

 //       return done;
	//}
}

/*
 * EX.
 * 
 * Unit steps.
 * Their weapon procs to hit adjacent enemy units.
 * The step command which normally would've just moved them now needs to be replaced w/ a bunch
 * of hitting, with a step kind of happening along the way.
 * 
 * So "instigating command" should be kinda replaceable.
 * Can set it null, or flip a bool to show it's been layered over top of w/ other reactions.
 * 
 * Can instigating command always be a single thing, for simplicity's sake..?
 */

/*
 * EX.
 * 
 * Unit moves one cell.
 * That unit has an item that procs some bashes, knocking back other units that are passed.
 * 
 * ProcessReactions()
 *  - BashCommands are generated. Now they sit in commands().
 *  - BashCommands will play anims & reaction effects, instigator will animate through them.
 *  
 * ProcessImpacts()
 *  - HitCommands (?) are generated. Now they sit in commands, offset slightly in time from their bashes.
 *  - HitCommands will actually dish damage or trigger resistances, numbers float, UI updates etc.
 *  
 * ProcessMovement()
 *  - All IMoving commands are considered within the TimeBlock. Any conflicts are caught & handled.
 *  - If movement works:
 *      
 *      PushCommands (?) are generated. Now they sit in commands, offset slighty from their hits.
 *      
 *      else
 *      
 *      BumpCommands (?) are generated. Now they sit in commands, offset slightly from their hits.
 *      
 * Can movements generated here proc MORE reactions...?
 * 
 * Maybe.
 * 
 * Strong argument for throttling how many times something can be proc'd within a Timeblock / Turn.
 * Ready / unready status for units capable of reacting.
 * 
 * Showing speed deltas.
 * 
 * 
 * so, ex:
 * 
 * Player wants move past several unit.
 * We hover the input, it shows the previews.
 * Those are handled by the item itself.
 * 
 * 
 * 
 * - TODO: movement type, active (procs movement-based stuff) or passive (doesn't)
 * 
 */


//void StartProcessingCommands(Queue<UnitCommand> commands)
//{
//    currInstigator = selectedUnit;
//    DeselectUnit();

//    commandsQueueToProcess = commands;
//    //currCommand = commandsToProcess.Dequeue();
//    currCommandStep = new UnitCommandStep(currInstigator, commandsQueueToProcess.Dequeue());

//    currCommandStep.instigatingCommand.OnBeginTick();
//    //currCommand.OnBeginTick();

//    //Board.RespondToCommandBeginTick(currInstigator, currCommand);

//    currCommandHistory = new Stack<UnitCommand>();
//    currCommandStepHistory = new Stack<UnitCommandStep>();

//    mode = Mode.ProcessingCommands;
//    playbackState = TurnPlaybackState.PLAYING;

//    Debug.Log("STARTED PROCESSING COMMANDS");
//}


//private void HandleTurnForward()
//{
//    if (currCommandStep == null)
//        return;

//    if (currCommandStep.Tick())
//    {
//        currCommandStep.CompleteTick();
//        currCommandStep.Execute();
//        currCommandStepHistory.Push(currCommandStep);

//        //Board.RespondToCommandCompleteTick(currInstigator, currCommand);

//        currTimeStep++;
//        Board.currTimeStep++;

//        //         if (currCommand.StepsTimeForward())
//        //{
//        //             currTimeStep++;
//        //             Board.currTimeStep++;
//        //}

//        //TODO: 
//        //... check that the next "intended" command step is valid, not
//        //... disrupted by the new board state

//        currCommandStep = null;

//        if (!commandsQueueToProcess.IsNullOrEmpty())
//        {
//            currCommandStep = new UnitCommandStep(currInstigator, commandsQueueToProcess.Dequeue());
//            currCommandStep.BeginTick();
//            //currCommand = commandsToProcess.Dequeue();
//            //currCommand.OnBeginTick();
//        }
//        else
//        {
//            playbackState = TurnPlaybackState.PAUSED;

//            TurnV2 recordedTurn = new();

//            recordedTurn.instigator = currInstigator;
//            recordedTurn.commandStepHistory = currCommandStepHistory;
//            //recordedTurn.commandHistory = currCommandHistory;
//            turnHistory.Push(recordedTurn);

//            currCommandStepHistory = null;
//            //currCommandHistory = null;
//        }
//    }
//}