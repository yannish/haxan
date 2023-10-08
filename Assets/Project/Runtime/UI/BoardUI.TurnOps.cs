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

    public Turn[] turns = new Turn[MAX_TURNS];
    public TurnStep[] turnSteps = new TurnStep[MAX_TURN_STEPS];

    public IUnitOperable[] allOps = new IUnitOperable[MAX_OPS];
    public IUnitOperable[] currInstigatingOps = new IUnitOperable[MAX_OPS];

    [ReadOnly] public int turnCount;
    [ReadOnly] public int turnStepCount;

    [ReadOnly] public int totalOps;
    [ReadOnly] public int totalInstigatingOps;

    [Header("PLAYBACK:")]
    [ReadOnly] public TurnPlaybackState playbackState;
    //... 
    [ReadOnly] public int currPlaybackStep;
    [ReadOnly] public int currPlaybackTurn;

    [ReadOnly] public float currNormalizedTime; //... reset to 0 as each TimeStep is processed.
    [ReadOnly] public float currPlaybackTime;   //... total running-time for the Turn
    [ReadOnly] public float prevPlaybackTime;
    [ReadOnly] public float currTimeScale = 1f;


    //... 

    void StartProcessingTurn(List<IUnitOperable> instigatingOps)
    {
        if (logTurnDebug)
            Debug.LogWarning("Starting to process unit ops.");

        currInstigator = selectedUnit;
        DeselectUnit();

        mode = Mode.ProcessingCommands;
        playbackState = TurnPlaybackState.PLAYING;

        //currNormalizedTime = 0f;
        currTimeScale = 1f;

        totalInstigatingOps = instigatingOps.Count;
		for (int i = 0; i < instigatingOps.Count; i++)
		{
            currInstigatingOps[i] = instigatingOps[i];
		}

        ProcessReactions();
    }

    /// <summary>
    /// for now this is just taking in the instigating ops
    /// ... & putting them directly into turnSteps.
    /// ... but really this is where you'd build more complex result from input action.
    /// </summary>
    void ProcessReactions()
    {
        int opIndex = totalOps;
		for (int i = totalOps; i < opIndex + totalInstigatingOps; i++)
		{
            allOps[i] = currInstigatingOps[i];
            totalOps++;

            TurnStep newTurnStep = new TurnStep()
            {
                opIndex = i,
                opCount = 1
            };

            turnSteps[turnStepCount] = newTurnStep;
            turnStepCount++;
		}

        Turn newTurn = new Turn()
        {
            instigator = currInstigator,
            stepCount = totalInstigatingOps
        };
        turns[turnCount] = newTurn;
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

    //Turn currTurn;
    //int currTurnIndex;

    TurnStep currTurnStep;
    int currTurnStepIndex;

    public void HandleTurnForward()
	{
        Turn currTurn = turns[currPlaybackTurn];
		for (int i = currTurn.stepIndex; i < currTurn.stepIndex + currTurn.stepCount; i++)
		{
            TurnStep currTurnStep = turnSteps[i];
			for (int j = currTurnStep.opIndex; j < currTurnStep.opIndex + currTurnStep.opCount; j++)
			{
                bool allOpsFullyTicked = true;

                IUnitOperable op = allOps[j];
                if (currPlaybackTime < op.data.startTime)
                    continue;

                Unit affectedUnit = op.data.unitIndex.ToUnit();

                //... BEGIN TICK:
                if (currPlaybackTime >= op.data.startTime && prevPlaybackTime < op.data.startTime)
				{
                //... OnBeginTick();    
				}

                //.. TICK:
                if(currPlaybackTime >= op.data.startTime && currPlaybackTime < op.data.endTime)
				{
                    allOpsFullyTicked = false;
                    var normalizedTime = Mathf.Clamp01(currPlaybackTime - op.data.startTime) / op.data.duration;
                    op.Tick(affectedUnit, normalizedTime);
				}

                //... COMPLETE TICK();
                if (currPlaybackTime > op.data.endTime && prevPlaybackTime < op.data.endTime)
				{
                    op.Execute(affectedUnit);
				}

                //... if we're at the final op of the final step:
                if(
                    allOpsFullyTicked
                    && i == (currTurn.stepIndex + currTurn.stepCount - 1)
                    && j == (currTurnStep.opIndex + currTurnStep.opCount - 1)
                    )
				{
                    Debug.Log("Played through all turnSteps.");

                    playbackState = TurnPlaybackState.PAUSED;
                    SelectUnit(lastSelectedUnit);
                    currInstigator = null;
                    currPlaybackTurn++;
                }
			}
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
