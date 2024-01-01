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
/// A turn-step is collection of UnitOps, played out in parallel.
/// </summary>
[System.Serializable]
public struct TurnStep
{
    //public float startTime;
    //public float duration;

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
	[ReadOnly] public int targetPlaybackTurn;
    //[ReadOnly] public int currPlaybackTurn;

    [ReadOnly] public float currNormalizedTime; //... reset to 0 as each TimeStep is processed.
    [ReadOnly] public float lastTurnStartTime;
    //[ReadOnly] public float Haxan.history.currPlaybackNewTime;   //... total running-time for the Turn
    //[ReadOnly] public float Haxan.history.prevPlaybackNewTime;
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
		if (Haxan.history.currPlaybackTurn == newTargetTurn)
		{
			Debug.LogWarning($"Already scrubbed to turn {newTargetTurn}!");
			return;
		}

		if(newTargetTurn > Haxan.history.turnCount)
		{
			Debug.LogWarning($"Target turn {newTargetTurn} out of bounds!");
			return;
		}

		lastSelectedUnit = selectedUnit;
		DeselectUnit();

		Debug.LogWarning($"Scrubbing to turn: {newTargetTurn}");

		mode = Mode.ProcessingCommands;
		if(newTargetTurn < Haxan.history.currPlaybackTurn)
		{
			SetStateToRewind();
		}
		else
		{
			SetStateToFastForward();
		}

		targetPlaybackTurn = newTargetTurn;
	}

	void SetStateToRewind()
	{
		playbackState = TurnPlaybackState.REWINDING;
		Haxan.history.currPlaybackTurn--;
		var currTurn = Haxan.history.currTurn;
		Haxan.history.currPlaybackTime = currTurn.endTime;
		Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime + 1f;
	}

	void SetStateToFastForward()
	{
		playbackState = TurnPlaybackState.FAST_FORWARDING;
		var currTurn = Haxan.history.currTurn;
		Haxan.history.currPlaybackTime = currTurn.startTime;
		Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime - 1f;
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

    void GenerateTurn()
    {
		float turnEndTime = 0f;
		float turnStartTime = Haxan.history.currPlaybackTurn == 0 ? 0f : Haxan.history.prevTurn.endTime;

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


		//var currTurn = Haxan.history.currTurn;
		//var currStep = Haxan.history.turnSteps[currTurn.stepIndex + currTurn.stepCount - 1];

		int indexToCreateStepsAt = Haxan.history.stepHead;
		//int indexToCreateStepsAt = currTurn.stepIndex + currTurn.stepCount;
		int indexToCreateOpsAt = Haxan.history.opHead;
		//int indexToCreateOpsAt = currStep.opIndex + currStep.opCount;

		Debug.LogWarning($"steps at {indexToCreateStepsAt}, ops at {indexToCreateOpsAt}");

		//int indexToCreateStepsAt = Haxan.history.totalCreatedTurnSteps;
		int numOpsCreatedThisTurn = 0; //... we track this for an index into the array of all Ops
		int numStepsCreatedThisTurn = 0;

		//... now write back all the List of generated ops over to the arrays:
		for (int i = 0; i < allGeneratedOps.Count; i++)
		{
			//... each list of ops converts to a new TurnStep:
			List<UnitOp> generatedOps = allGeneratedOps[i];
			TurnStep newTurnStep = new TurnStep()
			{
				opIndex = indexToCreateOpsAt + numOpsCreatedThisTurn,
				opCount = generatedOps.Count
			};

			int numOpsCreatedThisStep = 0;
			for (int j = 0; j < generatedOps.Count; j++)
			{
				Haxan.history.allOps[indexToCreateOpsAt + numOpsCreatedThisStep + numOpsCreatedThisTurn] = generatedOps[j];

				Debug.LogWarning($"new op created at {indexToCreateOpsAt + numOpsCreatedThisStep + numOpsCreatedThisTurn}");

				//Haxan.history.allOps[Haxan.history.totalCreatedOps + numOpsCreatedThisStep] = generatedOps[j];
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
			Haxan.history.turnSteps[Haxan.history.stepHead + numStepsCreatedThisTurn] = newTurnStep;

			Debug.LogWarning($"new step created at {Haxan.history.stepHead + numStepsCreatedThisTurn}");
			//Haxan.history.turnSteps[Haxan.history.totalCreatedTurnSteps] = newTurnStep;

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

		Haxan.history.turns[Haxan.history.currPlaybackTurn] = newTurn;
		//Haxan.history.currPlaybackTurn++;
		//Haxan.history.turnCount++;

		/*
		 * We've finished constructing a new turn. This now becomes the latest entry in the history.
		 * 
		 */
	}


	void HandleCommandProcessing()
	{
		switch (playbackState)
		{
			case TurnPlaybackState.PAUSED:
				break;

			case TurnPlaybackState.PLAYING:
				if (HandleTurnForward())
				{
					//... we're exiting PLAY mode, ie. we've input a new turn.
					//..  & we're creating history, not scrubbing through it.
					//..	so we clamp turnCount to the current playback turn. 
					//... other turns beyond this one are now lost in the playback options.

					playbackState = TurnPlaybackState.PAUSED;

					SelectUnit(lastSelectedUnit);
					currInstigator = null;

					Haxan.history.currPlaybackTurn++;
					Haxan.history.turnCount = Haxan.history.currPlaybackTurn;

					Haxan.history.currPlaybackTime = Haxan.history.currPlaybackTurn == 0
						? 0f
						: Haxan.history.prevTurn.endTime;

					Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime - Time.deltaTime * currTimeScale;
				}
				break;

			case TurnPlaybackState.REWINDING:
				if (HandleTurnBackward())
				{
					Debug.Log("Reversed through all turnSteps.");

					if (targetPlaybackTurn == Haxan.history.currPlaybackTurn)
					{
						playbackState = TurnPlaybackState.PAUSED;
						SelectUnit(lastSelectedUnit);

						currInstigator = null;
						Haxan.history.currPlaybackTime = Haxan.history.currTurn.startTime;
						Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime - Time.deltaTime * currTimeScale;
					}
					else
					{
						Haxan.history.currPlaybackTurn--;
					}
				}
				break;

			case TurnPlaybackState.FAST_FORWARDING:
				if (HandleTurnForward())
				{
					//... we're exiting FF mode, ie. we're just scrubbing around, & not creating history.
					//... other turns beyond this one are still shown in the playback options.

					Haxan.history.currPlaybackTurn++;
					if (targetPlaybackTurn == Haxan.history.currPlaybackTurn)
					{
						playbackState = TurnPlaybackState.PAUSED;
						SelectUnit(lastSelectedUnit);

						currInstigator = null;
						Haxan.history.currPlaybackTime = Haxan.history.prevTurn.endTime;
						Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime - Time.deltaTime * currTimeScale;
					}
				}
				break;

			default:
				break;
		}
	}

	bool HandleTurnForward()
	{
		Turn currTurn = Haxan.history.currTurn;

		bool allOpsFullyTicked = true;

		//... looping through all turnSteps in this current turn:
		for (int i = currTurn.stepIndex; i < currTurn.stepIndex + currTurn.stepCount; i++)
		{
			TurnStep currTurnStep = Haxan.history.turnSteps[i];

			//... looping through all ops in that step:
			for (int j = currTurnStep.opIndex; j < currTurnStep.opIndex + currTurnStep.opCount; j++)
			{
				UnitOp op = Haxan.history.allOps[j];
				OpPlaybackData opData = op.playbackData;

				float effectiveStartTime = opData.startTime + currTurn.startTime;
				float effectiveEndTime = opData.endTime + currTurn.startTime;

				if (Haxan.history.prevPlaybackTime < effectiveEndTime)
					//... if prevTime is less than effTime, this op will hasn't been ticked to completion, so:
					allOpsFullyTicked = false;

				if (Haxan.history.currPlaybackTime < effectiveStartTime)
				{
					//... our time isn't caught up to this Op yet:
					if (debugOps)
						Debug.Log($"out, currPlayback is pre startTime, {Haxan.history.currPlaybackTime}, {effectiveStartTime}");
					continue;
				}

				//if (Haxan.history.currPlaybackTime > effectiveEndTime && Haxan.history.prevPlaybackTime > effectiveEndTime)
				//	continue;

				if (debugOps)
					Debug.Log(
						$"forward ticking op: {j}, " +
						$"currPlaybackTime: {Haxan.history.currPlaybackTime}, " +
						$"prevPlaybackTime: {Haxan.history.prevPlaybackTime}, " +
						$"startTime: {effectiveStartTime}" +
						$"endTime: {effectiveEndTime}"
						);

				Unit affectedUnit = op.playbackData.unitIndex.ToUnit();
				//... BEGIN TICK:
				if (Haxan.history.currPlaybackTime >= effectiveStartTime && Haxan.history.prevPlaybackTime < effectiveStartTime)
				{
					//... OnBeginTick();    
				}

				//.. TICK:
				if (Haxan.history.currPlaybackTime >= effectiveStartTime && Haxan.history.currPlaybackTime < effectiveEndTime)
				{
					if (debugOps)
						Debug.LogWarning($"op {j} still running.");

					//allOpsFullyTicked = false;
					var normalizedTime = Mathf.Clamp01((Haxan.history.currPlaybackTime - effectiveStartTime) / op.playbackData.duration);
					op.Tick(affectedUnit, normalizedTime);
				}

				//... COMPLETE TICK();
				bool opCompletedThisFrame = Haxan.history.currPlaybackTime > effectiveEndTime && Haxan.history.prevPlaybackTime < effectiveEndTime;
				if (opCompletedThisFrame)
				{
					op.Tick(affectedUnit, 1f);
					op.Execute(affectedUnit);
				}
			}
		}

		Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime;
		Haxan.history.currPlaybackTime += Time.deltaTime * currTimeScale;

		//... if we're at the final op of the final step:
		if (allOpsFullyTicked)
		{
			Debug.Log("Played through all turnSteps.");

			//playbackState = TurnPlaybackState.PAUSED;
			//SelectUnit(lastSelectedUnit);

			//currInstigator = null;
			//Haxan.history.currPlaybackTurn++;

			////... we're exiting PLAY mode, ie. we've input a new turn.
			////..  & we're creating history, not scrubbing through it.
			////..	so we clamp turnCount to the current playback turn. 
			////... other turns beyond this one are now lost in the playback options.

			//Haxan.history.turnCount = Haxan.history.currPlaybackTurn;
			////Haxan.history.turnCount = Mathf.Clamp(Haxan.history.turnCount, 0, Haxan.history.currPlaybackTurn);

			////... TO-DO:
			////... should reset playback time here somehow, avoid grimey numbers:

			//Haxan.history.currPlaybackNewTime = Haxan.history.currPlaybackTurn == 0
			//	? 0f
			//	: Haxan.history.prevTurn.endTime;

			return true;
		}

		return false;
	}

    void HandleTurnForward_OLDER()
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

				if (Haxan.history.prevPlaybackTime < effectiveEndTime)
				//... if prevTime is less than effTime, this op will hasn't been ticked to completion, so:
					allOpsFullyTicked = false;

				if (Haxan.history.currPlaybackTime < effectiveStartTime)
				{
					if(debugOps)
						Debug.Log($"out, currPlayback is pre startTime, {Haxan.history.currPlaybackTime}, {effectiveStartTime}");
					//... our time isn't caught up to this Op yet:
					continue;
				}

                Unit affectedUnit = op.playbackData.unitIndex.ToUnit();

                //... BEGIN TICK:
                if (Haxan.history.currPlaybackTime >= effectiveStartTime && Haxan.history.prevPlaybackTime < effectiveStartTime)
				{
					//... OnBeginTick();    
				}

				if (debugOps)
					Debug.Log(
						$"forward ticking op: {j}, " +
						$"Haxan.history.currPlaybackNewTime: {Haxan.history.currPlaybackTime}, " +
						$"Haxan.history.prevPlaybackNewTime: {Haxan.history.prevPlaybackTime}, " +
						$"startTime: {effectiveStartTime}" +
						$"endTime: {effectiveEndTime}"
						);

				//.. TICK:
				if (Haxan.history.currPlaybackTime >= effectiveStartTime && Haxan.history.currPlaybackTime < effectiveEndTime)
				{
					if(debugOps)
						Debug.LogWarning($"op {j} still running.");

                    //allOpsFullyTicked = false;
                    var normalizedTime = Mathf.Clamp01((Haxan.history.currPlaybackTime - effectiveStartTime) / op.playbackData.duration);
                    op.Tick(affectedUnit, normalizedTime);
				}

				bool opCompletedThisFrame = Haxan.history.currPlaybackTime > effectiveEndTime && Haxan.history.prevPlaybackTime < effectiveEndTime;
				//... COMPLETE TICK();
				if (opCompletedThisFrame)
				{
					op.Tick(affectedUnit, 1f);
                    op.Execute(affectedUnit);
				}
			}
		}

		Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime;
		Haxan.history.currPlaybackTime += Time.deltaTime * currTimeScale;

		//... if we're at the final op of the final step:
		if (allOpsFullyTicked)
		{
			Debug.Log("Played through all turnSteps.");

			playbackState = TurnPlaybackState.PAUSED;
			SelectUnit(lastSelectedUnit);

			currInstigator = null;
			Haxan.history.currPlaybackTurn++;

			//... we're exiting PLAY mode, ie. we've input a new turn.
			//..  & we're creating history, not scrubbing through it.
			//..	so we clamp turnCount to the current playback turn. 
			//... other turns beyond this one are now lost in the playback options.

			Haxan.history.turnCount = Haxan.history.currPlaybackTurn;
			//Haxan.history.turnCount = Mathf.Clamp(Haxan.history.turnCount, 0, Haxan.history.currPlaybackTurn);

			//... TO-DO:
			//... should reset playback time here somehow, avoid grimey numbers:

			Haxan.history.currPlaybackTime = Haxan.history.currPlaybackTurn == 0
				? 0f
				: Haxan.history.prevTurn.endTime;
				//Haxan.history.turns[currPlaybackTurn].endTime;
		}
    }

	bool HandleTurnBackward()
	{
		Turn currTurn = Haxan.history.currTurn;

		bool allOpsFullyUnticked = true;

		//... looping through all turnSteps:
		for (int i = currTurn.stepIndex + currTurn.stepCount - 1; i >= currTurn.stepIndex; i--)
		{
			TurnStep currTurnStep = Haxan.history.turnSteps[i];

			//NOTE: why is it turn & move playing together in same frame & failing...

			//... looping through all ops in that step:
			for (int j = currTurnStep.opIndex + currTurnStep.opCount - 1; j >= currTurnStep.opIndex; j--)
			{
				UnitOp op = Haxan.history.allOps[j];
				OpPlaybackData opData = op.playbackData;

				float opStartTime = opData.startTime + currTurn.startTime;
				float opEndTime = opData.endTime + currTurn.startTime;

				if (Haxan.history.prevPlaybackTime > opStartTime)
					allOpsFullyUnticked = false;

				if (Haxan.history.currPlaybackTime > opEndTime)
				//if (Haxan.history.currPlaybackNewTime < opStartTime)
				{
					Debug.Log($"out, currPlayback is post endTime, {Haxan.history.currPlaybackTime}, {opStartTime}");
					continue;
				}

				Unit affectedUnit = op.playbackData.unitIndex.ToUnit();

				// TODO: offet opDataStartTime by the startTime of the turn you're looking at.

				//... BEGIN TICK:
				if (Haxan.history.currPlaybackTime >= opStartTime && Haxan.history.prevPlaybackTime < opStartTime)
				{
					//... OnBeginTick();    
				}

				Debug.Log(
					$"backwards op: {j}, " +
					$"curr: {Haxan.history.currPlaybackTime}, " +
					$"prev: {Haxan.history.prevPlaybackTime}, " +
					$"S: {opStartTime}, " +
					$"E: {opEndTime}"
					);

				//.. TICK:
				if (Haxan.history.currPlaybackTime >= opStartTime && Haxan.history.currPlaybackTime < opEndTime)
				{
					Debug.LogWarning($"op {j} still backwards running.");
					//allOpsFullyTicked = false;
					var normalizedTime = Mathf.Clamp01((Haxan.history.currPlaybackTime - opStartTime) / op.playbackData.duration);
					op.Tick(affectedUnit, normalizedTime);
				}

				bool opUndoneThisFrame = Haxan.history.currPlaybackTime <= opStartTime && Haxan.history.prevPlaybackTime > opStartTime;
				//... COMPLETE TICK();
				if (opUndoneThisFrame)
				{
					Debug.LogWarning($"... op {j} undone.");
					op.Tick(affectedUnit, 0f);
					op.Undo(affectedUnit);
				}
			}
		}

		Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime;
		Haxan.history.currPlaybackTime -= Time.deltaTime * currTimeScale;

		Debug.Log(
					$"... curr: {Haxan.history.currPlaybackTime}, " +
					$"... prev: {Haxan.history.prevPlaybackTime} "
					);

		//... if we're at the final op of the final step:
		if (allOpsFullyUnticked)
		{
			return true;
		}

		return false;
	}

	void HandleTurnBackward_OLD()
	{
		Turn currTurn = Haxan.history.currTurn;

		bool allOpsFullyUnticked = true;

		//... looping through all turnSteps:
		for (int i = currTurn.stepIndex + currTurn.stepCount - 1; i >= currTurn.stepIndex; i--)
		{
			TurnStep currTurnStep = Haxan.history.turnSteps[i];

			//NOTE: why is it turn & move playing together in same frame & failing...

			//... looping through all ops in that step:
			for (int j = currTurnStep.opIndex + currTurnStep.opCount - 1; j >= currTurnStep.opIndex; j--)
			{
				UnitOp op = Haxan.history.allOps[j];
				OpPlaybackData opData = op.playbackData;

				float opStartTime = opData.startTime + currTurn.startTime;
				float opEndTime = opData.endTime + currTurn.startTime;

				if (Haxan.history.prevPlaybackTime > opStartTime)
					allOpsFullyUnticked = false;

				if (Haxan.history.currPlaybackTime > opEndTime)
				//if (Haxan.history.currPlaybackNewTime < opStartTime)
				{
					Debug.Log($"out, currPlayback is post endTime, {Haxan.history.currPlaybackTime}, {opStartTime}");
					continue;
				}

				Unit affectedUnit = op.playbackData.unitIndex.ToUnit();

				// TODO: offet opDataStartTime by the startTime of the turn you're looking at.

				//... BEGIN TICK:
				if (Haxan.history.currPlaybackTime >= opStartTime && Haxan.history.prevPlaybackTime < opStartTime)
				{
					//... OnBeginTick();    
				}

				Debug.Log(
					$"backwards op: {j}, " +
					$"curr: {Haxan.history.currPlaybackTime}, " +
					$"prev: {Haxan.history.prevPlaybackTime}, " +
					$"S: {opStartTime}, " +
					$"E: {opEndTime}"
					);

				//.. TICK:
				if (Haxan.history.currPlaybackTime >= opStartTime && Haxan.history.currPlaybackTime < opEndTime)
				{
					Debug.LogWarning($"op {j} still backwards running.");
					//allOpsFullyTicked = false;
					var normalizedTime = Mathf.Clamp01((Haxan.history.currPlaybackTime - opStartTime) / op.playbackData.duration);
					op.Tick(affectedUnit, normalizedTime);
				}

				bool opUndoneThisFrame = Haxan.history.currPlaybackTime <= opStartTime && Haxan.history.prevPlaybackTime > opStartTime;
				//... COMPLETE TICK();
				if (opUndoneThisFrame)
				{
					Debug.LogWarning($"... op {j} undone.");
					op.Tick(affectedUnit, 0f);
					op.Undo(affectedUnit);
				}
			}
		}

		Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime;
		Haxan.history.currPlaybackTime -= Time.deltaTime * currTimeScale;

		Debug.Log(
					$"... curr: {Haxan.history.currPlaybackTime}, " +
					$"... prev: {Haxan.history.prevPlaybackTime} " 
					);

		//... if we're at the final op of the final step:
		if (allOpsFullyUnticked)
		{
			Debug.Log("Reversed through all turnSteps.");

			if(targetPlaybackTurn == Haxan.history.currPlaybackTurn)
			{
				playbackState = TurnPlaybackState.PAUSED;
				SelectUnit(lastSelectedUnit);

				currInstigator = null;
				Haxan.history.currPlaybackTime = currTurn.startTime;
				Haxan.history.prevPlaybackTime = Haxan.history.currPlaybackTime - Time.deltaTime * currTimeScale;
			}
			else
			{
				Haxan.history.currPlaybackTurn--;
			}
		}
	}

	void HandleFastForward()
	{

	}
}
