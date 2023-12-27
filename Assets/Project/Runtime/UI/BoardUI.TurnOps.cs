using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnPlaybackState
{
    PAUSED,
    PLAYING,
    REWINDING,
	FAST_FORWARDING
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
    public const int MAX_OPS = 500;
    public const int MAX_INSTIGATING_OPS = 100;
    public const int MAX_TURNS = 100;
    public const int MAX_TURN_STEPS = 100;

	//public Turn[] turns = new Turn[MAX_TURNS];
	//public TurnStep[] turnSteps = new TurnStep[MAX_TURN_STEPS];
	//public IUnitOperable[] allOps = new IUnitOperable[MAX_OPS];

	//[ReadOnly] public int turnCount;
	//[ReadOnly] public int totalCreatedTurnSteps;
	//[ReadOnly] public int totalCreatedOps;

	public UnitOp_STRUCT[] allOps_NEW = new UnitOp_STRUCT[MAX_OPS];
    public List<UnitOp_STRUCT> currInstigatingOps_NEW;
    public List<IUnitOperable> currInstigatingOps_OLD;
	public List<UnitOp> currInstigatingOps;

	//public IUnitOperable[] currInstigatingOps = new IUnitOperable[MAX_OPS];

	//[ReadOnly] public int totalInstigatingOps;

	[Header("PLAYBACK:")]
    [ReadOnly] public TurnPlaybackState playbackState;
    //... 
    [ReadOnly] public int currPlaybackStep;
    //[ReadOnly] public int currPlaybackTurn;

	[ReadOnly] public int targetPlaybackTurn;

    [ReadOnly] public float currNormalizedTime; //... reset to 0 as each TimeStep is processed.
    [ReadOnly] public float lastTurnStartTime;
    [ReadOnly] public float currPlaybackTime;   //... total running-time for the Turn
    [ReadOnly] public float prevPlaybackTime;
    [ReadOnly] public float currTimeScale = 1f;

	[Header("DUMMY:")]
	public int numDummyOps;

	void OnEnable()
	{
		GameContext.OnScrubToTurnClicked += HandleScrubToTurn;
		GameContext.OnSmashToTurnClicked += HandleSmashCutToTurn;
	}

	void OnDisable()
	{
		GameContext.OnScrubToTurnClicked -= HandleScrubToTurn;
		GameContext.OnSmashToTurnClicked -= HandleSmashCutToTurn;
	}

	void HandleScrubToTurn(int newTargetTurn)
	{
		Debug.LogWarning($"Scrubbing to turn: {newTargetTurn}");
		if (Haxan.history.currPlaybackTurn == newTargetTurn)
			return;

		mode = Mode.ProcessingCommands;
		playbackState = newTargetTurn < Haxan.history.currPlaybackTurn 
			? TurnPlaybackState.REWINDING 
			: TurnPlaybackState.FAST_FORWARDING;
		targetPlaybackTurn = newTargetTurn;
	}

	void HandleSmashCutToTurn(int turnIndex)
	{

	}

    void StartProcessingTurn(List<UnitOp> instigatingOps)
    {
        if (logTurnDebug)
            Debug.LogWarning("Starting to process unit ops.");

        currInstigator = selectedUnit;
        DeselectUnit();

        mode = Mode.ProcessingCommands;
        playbackState = TurnPlaybackState.PLAYING;

        currTimeScale = 1f;

        this.currInstigatingOps = instigatingOps;

		GenerateTurn();
    
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
    void GenerateTurn()
    {
		float turnEndTime = 0f;
		float turnStartTime = Haxan.history.turnCount == 0 ? 0f : Haxan.history.prevTurn.endTime;

		//... this would have to catch implicit interruptions, like causing death :
		List<UnitOp> CheckForInterrupt(List<UnitOp> inputOps)
		{
            List<UnitOp> filteredOps = new List<UnitOp>();
            foreach(var op in inputOps)
			{
                foreach(var unit in Haxan.activeUnits.Items)
				{
                    foreach(var ability in unit.Abilities)
					{
                        var interruption = ability.TryInterrupOp(currInstigator, op);
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

        var interruptFilteredOps = CheckForInterrupt(currInstigatingOps);


		List<UnitOp> ProcessOpForReactions(UnitOp op)
		{
			//... every instigating op will create a list, containing either itself + reactions, or simply itself:
			List<UnitOp> turnStepOps = new List<UnitOp>();

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

		List<List<UnitOp>> allGeneratedOps = new List<List<UnitOp>>();
		for (int i = 0; i < interruptFilteredOps.Count; i++)
		{
			var generatedOps = ProcessOpForReactions(interruptFilteredOps[i]);
			allGeneratedOps.Add(generatedOps);
		}


		int indexToCreateStepsAt = Haxan.history.totalCreatedTurnSteps;
		int numOpsCreatedThisTurn = 0; //... we track this for an index into the array of all Ops
		int numStepsCreatedThisTurn = 0; 

		//... now write back all the List of generated ops over to the arrays:
		for (int i = 0; i < allGeneratedOps.Count; i++)
		{
			//... each list of ops converts to a new TurnStep:
			List<UnitOp> generatedOps = allGeneratedOps[i];
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
				var realEndTime = generatedOps[j].playbackData.endTime + turnStartTime;
				if (realEndTime > turnEndTime)
					turnEndTime = generatedOps[j].playbackData.endTime + turnStartTime;
			}

			for (int k = 0; k < numOpsCreatedThisTurn; k++)
			{

			}

			Haxan.history.totalCreatedOps += numOpsCreatedThisStep;
			Haxan.history.turnSteps[Haxan.history.totalCreatedTurnSteps] = newTurnStep;
			
			numStepsCreatedThisTurn++;
			Haxan.history.totalCreatedTurnSteps++;
		}

		Turn newTurn = new Turn()
		{
			instigator = currInstigator,
			stepIndex = indexToCreateStepsAt,
			stepCount = numStepsCreatedThisTurn,

			//... TODO: this should instead be some lastTurnEndTime:
			startTime = turnStartTime,
			endTime = turnEndTime
		};

		Haxan.history.turns[Haxan.history.turnCount] = newTurn;
		Haxan.history.turnCount++;
    }

	void HandleCommandProcessing()
	{
		switch (playbackState)
		{
			case TurnPlaybackState.PAUSED:
				break;

			case TurnPlaybackState.PLAYING:
				HandleTurnForward();
				break;

			case TurnPlaybackState.REWINDING:
                HandleTurnBackward();
				break;

			default:
				break;
		}
	}

    void HandleTurnForward()
	{
        Turn currTurn = Haxan.history.currTurn;
		/*. every tick, we run through every TURN STEP
		 * ... even ones that are out of range
		 * ... so we're always getting to those "end" indices after running a full loop
		 *.... and so on final run through the loop, allOpsFullyTicked is reset, but never flipped false 
		 *.... for that out-of-range op
		 *.... 
		 *.... why is this currently working for the FIRST turn though...
		*/

		bool allOpsFullyTicked = true;

		//... looping through all turnSteps in this current turn:
		for (int i = currTurn.stepIndex; i < currTurn.stepIndex + currTurn.stepCount; i++)
		{
			TurnStep currTurnStep = Haxan.history.turnSteps[i];

			//NOTE: why is it turn & move playing together in same frame & failing...

			//... looping through all ops in that step:
			for (int j = currTurnStep.opIndex; j < currTurnStep.opIndex + currTurnStep.opCount; j++)
			{
				UnitOp op = Haxan.history.allOps[j];
				OpPlaybackData opData = op.playbackData;

				float effectiveStartTime = opData.startTime + currTurn.startTime;
				float effectiveEndTime = opData.endTime + currTurn.startTime;

				if (prevPlaybackTime < effectiveEndTime)
				//... if prevTime is less than effTime, this op will hasn't been ticked to completion, so:
					allOpsFullyTicked = false;

				if (currPlaybackTime < effectiveStartTime)
				//... our time isn't caught up to this Op yet:
					continue;

                Unit affectedUnit = op.playbackData.unitIndex.ToUnit();

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
                    var normalizedTime = Mathf.Clamp01((currPlaybackTime - effectiveStartTime) / op.playbackData.duration);
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

		prevPlaybackTime = currPlaybackTime;
		currPlaybackTime += Time.deltaTime * currTimeScale;

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
			Haxan.history.currPlaybackTurn++;

			//... TO-DO:
			//... should reset playback time here somehow, avoid grimey numbers:

			//currPlaybackTime = Haxan.history.turns[currPlaybackTurn].endTime;
		}

		//prevPlaybackTime = currPlaybackTime;
  //      currPlaybackTime += Time.deltaTime * currTimeScale;
    }

	void HandleTurnBackward()
	{
		Turn currTurn = Haxan.history.currTurn;

		bool allOpsFullyUnticked = true;

		//... looping through all turnSteps:
		for (int i = currTurn.stepIndex + currTurn.stepCount - 1; i >= currTurn.stepIndex; i--)
		{
			TurnStep currTurnStep = Haxan.history.turnSteps[i];

			//NOTE: why is it turn & move playing together in same frame & failing...

			//... looping through all ops in that step:
			for (int j = currTurnStep.opIndex + currTurnStep.opCount - 1; j > currTurnStep.opIndex; j--)
			{
				UnitOp op = Haxan.history.allOps[j];
				OpPlaybackData opData = op.playbackData;

				float effectiveStartTime = opData.startTime + currTurn.startTime;
				float effectiveEndTime = opData.endTime + currTurn.startTime;

				if (prevPlaybackTime > effectiveStartTime)
					allOpsFullyUnticked = false;

				if (currPlaybackTime < effectiveStartTime)
					continue;

				Unit affectedUnit = op.playbackData.unitIndex.ToUnit();

				// TODO: offet opDataStartTime by the startTime of the turn you're looking at.

				//... BEGIN TICK:
				if (currPlaybackTime >= effectiveStartTime && prevPlaybackTime < effectiveStartTime)
				{
					//... OnBeginTick();    
				}

				Debug.Log(
					$"backwards ticking op: {j}, " +
					$"playbackTime: {currPlaybackTime}, " +
					$"startTime: {effectiveStartTime}" +
					$"endTime: {effectiveEndTime}"
					);

				//.. TICK:
				if (currPlaybackTime >= effectiveStartTime && currPlaybackTime < effectiveEndTime)
				{
					Debug.LogWarning($"op {j} still backwards running.");
					//allOpsFullyTicked = false;
					var normalizedTime = Mathf.Clamp01((currPlaybackTime - effectiveStartTime) / op.playbackData.duration);
					op.Tick(affectedUnit, normalizedTime);
				}

				bool opUndoneThisFrame = currPlaybackTime > effectiveEndTime && prevPlaybackTime < effectiveEndTime;
				//... COMPLETE TICK();
				if (opUndoneThisFrame)
				{
					op.Undo(affectedUnit);
				}
			}
		}

		prevPlaybackTime = currPlaybackTime;
		currPlaybackTime -= Time.deltaTime * currTimeScale;

		//... if we're at the final op of the final step:
		if (
			//opCompletedThisFrame
			allOpsFullyUnticked
			//&& i == (currTurn.stepIndex + currTurn.stepCount - 1)
			//&& j == (currTurnStep.opIndex + currTurnStep.opCount - 1)
			)
		{
			Debug.Log("Reversed through all turnSteps.");

			if(targetPlaybackTurn == Haxan.history.currPlaybackTurn)
			{
				playbackState = TurnPlaybackState.PAUSED;
				SelectUnit(lastSelectedUnit);

				currInstigator = null;
				Haxan.history.currPlaybackTurn--;
			}
			else
			{
				Haxan.history.currPlaybackTurn--;
			}
		}
	}
}
