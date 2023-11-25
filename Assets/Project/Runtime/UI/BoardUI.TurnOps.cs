using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnPlaybackState
{
    PAUSED,
    PLAYING,
    REWINDING
}


/// <summary>
/// A turn is a set of TurnSteps, played out in series.
/// </summary>
[System.Serializable]
public struct Turn
{
    public Unit instigator; //... have to find a way to serialize & then recreate this on the fly.

    public int stepIndex;
    public int stepCount;

    //.. mark this when you build the turn. then, all steps / op start times are relative...?
    public float startTime;
	public float endTime;

	public float duration => endTime - startTime;
}


/// <summary>
/// A turn-step is the basic building block of a Turn. 
/// It can contain any number of UnitOps, played out in parallel.
/// </summary>
[System.Serializable]
public struct TurnStep
{
    public float startTime;
    public float duration;

    public int opIndex;
    public int opCount;
}


public partial class BoardUI : MonoBehaviour
{
    public const int MAX_OPS = 100;
    public const int MAX_INSTIGATING_OPS = 100;
    public const int MAX_TURNS = 100;
    public const int MAX_TURN_STEPS = 100;

	//public Turn[] turns = new Turn[MAX_TURNS];
	//public TurnStep[] turnSteps = new TurnStep[MAX_TURN_STEPS];
	//public IUnitOperable[] allOps = new IUnitOperable[MAX_OPS];

	//[ReadOnly] public int turnCount;
	//[ReadOnly] public int totalCreatedTurnSteps;
	//[ReadOnly] public int totalCreatedOps;

	public UnitOp[] allOps_NEW = new UnitOp[MAX_OPS];
    public List<UnitOp> currInstigatingOps_NEW;
    public List<IUnitOperable> currInstigatingOps;

	//public IUnitOperable[] currInstigatingOps = new IUnitOperable[MAX_OPS];

    //[ReadOnly] public int totalInstigatingOps;

    [Header("PLAYBACK:")]
    [ReadOnly] public TurnPlaybackState playbackState;
    //... 
    [ReadOnly] public int currPlaybackStep;
    [ReadOnly] public int currPlaybackTurn;

    [ReadOnly] public float currNormalizedTime; //... reset to 0 as each TimeStep is processed.
    [ReadOnly] public float lastTurnStartTime;
    [ReadOnly] public float currPlaybackTime;   //... total running-time for the Turn
    [ReadOnly] public float prevPlaybackTime;
    [ReadOnly] public float currTimeScale = 1f;

	[Header("DUMMY:")]
	public int numDummyOps;


	void StartProcessingTurn_NEW(List<UnitOp> instigatingOps)
	{
		if (logTurnDebug)
			Debug.LogWarning("Starting to process unit ops.");

		currInstigator = selectedUnit;
		DeselectUnit();

		mode = Mode.ProcessingCommands;
		playbackState = TurnPlaybackState.PLAYING;

		//currNormalizedTime = 0f;
		currTimeScale = 1f;

		this.currInstigatingOps_NEW = instigatingOps;

		//      totalInstigatingOps = instigatingOps.Count;
		//for (int i = 0; i < instigatingOps.Count; i++)
		//{
		//          currInstigatingOps[i] = instigatingOps[i];
		//}

		ProcessReactions_NEW();
	}

    //... 
    void StartProcessingTurn(List<IUnitOperable> instigatingOps)
    {
        if (logTurnDebug)
            Debug.LogWarning("Starting to process unit ops.");

        currInstigator = selectedUnit;
        DeselectUnit();

        mode = Mode.ProcessingCommands;
        playbackState = TurnPlaybackState.PLAYING;

        currTimeScale = 1f;

        this.currInstigatingOps = instigatingOps;

		ProcessReactions();
    
		//currNormalizedTime = 0f;
  //      totalInstigatingOps = instigatingOps.Count;
		//for (int i = 0; i < instigatingOps.Count; i++)
		//{
  //          currInstigatingOps[i] = instigatingOps[i];
		//}
    }



    /// <summary>
    /// for now this is just taking in the instigating ops
    /// ... & putting them directly into turnSteps.
    /// ... but really this is where you'd build more complex result from input action.
    /// </summary>
    void ProcessReactions()
    {
		float turnEndTime = 0f;

		//... this would have to catch implicit interruptions, like causing death :
        List<IUnitOperable> CheckForInterrupt(List<IUnitOperable> inputOps)
		{
            List<IUnitOperable> filteredOps = new List<IUnitOperable>();
            foreach(var op in inputOps)
			{
                foreach(var unit in Haxan.activeUnits.Items)
				{
                    foreach(var ability in unit.Abilities)
					{
                        var interruption = ability.TryInterruptOp(currInstigator, op);
                        if (interruption == OpInterruptType.RETALIATE)
                        {
                            filteredOps.Add(op);
                            return filteredOps;
                        }

                        if (interruption == OpInterruptType.INTERDICT)
                            return filteredOps;
                    }
				}
                
				//... if we get here, the op was uninterrupted:
                filteredOps.Add(op);
			}

            return filteredOps;
		}

        var interruptCheckedOps = CheckForInterrupt(currInstigatingOps);


		List<IUnitOperable> ProcessOpForReactions(IUnitOperable op)
		{
			//... every instigating op will create a list, containing either itself + reactions, or simply itself:
			List<IUnitOperable> turnStepOps = new List<IUnitOperable>();

			//... currently only garnering ONE batch of reactions per op...
			//... otherwise they'd need more processing.
			bool foundReaction = false;
			foreach(var unit in Haxan.activeUnits.Items)
			{
				if (foundReaction)
					break;

				foreach (var ability in unit.Abilities)
				{
					if (foundReaction)
						break;

					if (ability.TryReact(op, out var reactions))
					{
						reactions.AddRange(reactions);
						foundReaction = true;
						continue;
					}
				}
			}

			if (!foundReaction)
				turnStepOps.Add(op);

			return turnStepOps;
		}

		List<List<IUnitOperable>> allGeneratedOps = new List<List<IUnitOperable>>();
		for (int i = 0; i < interruptCheckedOps.Count; i++)
		{
			var generatedOps = ProcessOpForReactions(interruptCheckedOps[i]);
			allGeneratedOps.Add(generatedOps);
		}

		int indexToCreateStepsAt = Haxan.history.totalCreatedTurnSteps;
		int numOpsCreatedThisTurn = 0; //... we track this for an index into the array of all Ops
		int numStepsCreatedThisTurn = 0; 

		//... now write back all the List of generated ops over to the arrays:
		for (int i = 0; i < allGeneratedOps.Count; i++)
		{
			//... each list of ops converts to a new TurnStep:
			List<IUnitOperable> generatedOps = allGeneratedOps[i];
			TurnStep newTurnStep = new TurnStep()
			{
				opIndex = Haxan.history.totalCreatedOps,
				opCount = generatedOps.Count
			};

			int numOpsCreatedThisStep = 0;
			for (int j = 0; j < generatedOps.Count; j++)
			{
				Haxan.history.allOps[Haxan.history.totalCreatedOps + numOpsCreatedThisStep] = generatedOps[j];
				numOpsCreatedThisStep++;
				numOpsCreatedThisTurn++;
				var realEndTime = generatedOps[j].data.endTime + currPlaybackTime;
				if (realEndTime > turnEndTime)
					turnEndTime = generatedOps[j].data.endTime + currPlaybackTime;
			}

			Haxan.history.totalCreatedOps += numOpsCreatedThisTurn;
			Haxan.history.turnSteps[Haxan.history.totalCreatedTurnSteps] = newTurnStep;
			
			numStepsCreatedThisTurn++;
			Haxan.history.totalCreatedTurnSteps++;

			//if()
		}

		Turn newTurn = new Turn()
		{
			instigator = currInstigator,
			stepIndex = indexToCreateStepsAt,
			stepCount = numStepsCreatedThisTurn,
			startTime = currPlaybackTime,
			endTime = turnEndTime
		};

		Haxan.history.turns[Haxan.history.turnCount] = newTurn;
		Haxan.history.turnCount++;

		//      //int opIndex = totalCreatedOps;
		//for (int i = 0; i < totalInstigatingOps; i++)
		//      {
		//          //.. we just add this op to the list of all ops:
		//          allOps[totalCreatedOps + i] = currInstigatingOps[i];
		//          TurnStep newTurnStep = new TurnStep()
		//          {
		//              opIndex = i + totalCreatedOps,
		//              opCount = 1
		//          };

		//          totalCreatedOps++;

		//          turnSteps[turnStepCount] = newTurnStep;
		//          turnStepCount++;
		//}

		//      Turn newTurn = new Turn()
		//      {
		//          instigator = currInstigator,
		//          stepCount = totalInstigatingOps,
		//          //startTime = 
		//      };

		//turns[turnCount] = newTurn;
  //      turnCount++;
    }

	void HandleCommandProcessing()
	{
		switch (playbackState)
		{
			case TurnPlaybackState.PAUSED:
				break;

			case TurnPlaybackState.PLAYING:
                //HandleTurnForward_NEW();
				HandleTurnForward();
				break;

			case TurnPlaybackState.REWINDING:
                HandleTurnBackward();
				break;

			default:
				break;
		}
	}

    //Turn currTurn;
    //int currTurnIndex;

    TurnStep currTurnStep;
    int currTurnStepIndex;

    public void HandleTurnForward()
	{
        Turn currTurn = Haxan.history.turns[currPlaybackTurn];
		/*. every tick, we run through every TURN STEP
		 * ... even ones that are out of range
		 * ... so we're always getting to those "end" indices after running a full loop
		 *.... and so on final run through the loop, allOpsFullyTicked is reset, but never flipped false 
		 *.... for that out-of-range op
		 *.... 
		 *.... why is this currently working for the FIRST turn though...
		*/

		bool allOpsFullyTicked = true;

		//... looping through all turnSteps:
		for (int i = currTurn.stepIndex; i < currTurn.stepIndex + currTurn.stepCount; i++)
		{
			TurnStep currTurnStep = Haxan.history.turnSteps[i];

			//NOTE: why is it turn & move playing together in same frame & failing...

			//... looping through all ops in that step:
			for (int j = currTurnStep.opIndex; j < currTurnStep.opIndex + currTurnStep.opCount; j++)
			{
				IUnitOperable op = Haxan.history.allOps[j];
				OpPlaybackData opData = op.data;

				float effectiveStartTime = opData.startTime + currTurn.startTime;
				float effectiveEndTime = opData.endTime + currTurn.startTime;

				if (prevPlaybackTime < effectiveEndTime)
					allOpsFullyTicked = false;

				if (currPlaybackTime < effectiveStartTime)
				//if (currPlaybackTime < op.data.startTime)
					continue;

                Unit affectedUnit = op.data.unitIndex.ToUnit();

				// TODO: offet opDataStartTime by the startTime of the turn you're looking at.

                //... BEGIN TICK:
                if (currPlaybackTime >= effectiveStartTime && prevPlaybackTime < effectiveStartTime)
				{
                //... OnBeginTick();    
				}

				Debug.Log(
					$"ticking op: {j}, " +
					$"playbackTime: {currPlaybackTime}, " +
					$"startTime: {effectiveStartTime}" +
					$"endTime: {effectiveEndTime}"
					);

				//.. TICK:
				if (currPlaybackTime >= effectiveStartTime && currPlaybackTime < effectiveEndTime)
				{
					Debug.LogWarning($"op {j} still running.");
                    //allOpsFullyTicked = false;
                    var normalizedTime = Mathf.Clamp01((currPlaybackTime - effectiveStartTime) / op.data.duration);
                    op.Tick(affectedUnit, normalizedTime);
				}

				bool opCompletedThisFrame = currPlaybackTime > effectiveEndTime && prevPlaybackTime < effectiveEndTime;
				//... COMPLETE TICK();
				if (opCompletedThisFrame)
				{
                    op.Execute(affectedUnit);
				}
			}
		}

		//... if we're at the final op of the final step:
		if (
			//opCompletedThisFrame
			allOpsFullyTicked
			//&& i == (currTurn.stepIndex + currTurn.stepCount - 1)
			//&& j == (currTurnStep.opIndex + currTurnStep.opCount - 1)
			)
		{
			Debug.Log("Played through all turnSteps.");

			playbackState = TurnPlaybackState.PAUSED;
			SelectUnit(lastSelectedUnit);
			currInstigator = null;
			currPlaybackTurn++;
		}

		prevPlaybackTime = currPlaybackTime;
        currPlaybackTime += Time.deltaTime * currTimeScale;
    }

    void ProcessImpacts()
    {

    }

    void ProcessMovement()
    {

    }

}
