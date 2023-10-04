using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A turn is a set of TurnSteps, played out in series.
/// </summary>
[System.Serializable]
public struct Turn
{
    public Unit instigator; //... have to find a way to serialize & then recreate this on the fly.

    public int index;
    public int totalSteps;
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
    public int index;
    public int totalOps;
}


public partial class BoardUI : MonoBehaviour
{
    public const int MAX_OPS = 100;
    public const int MAX_INSTIGATING_OPS = 100;
    public const int MAX_TURNS = 100;
    public const int MAX_TURN_STEPS = 100;

    public Turn[] turns = new Turn[MAX_TURNS];
    public TurnStep[] turnSteps = new TurnStep[MAX_TURN_STEPS];
    public UnitOp[] allOps = new UnitOp[MAX_OPS];
    public UnitOp[] currInstigatingOps = new UnitOp[MAX_OPS];

    [ReadOnly] public int currTurn;
    [ReadOnly] public int currTurnStep;
    [ReadOnly] public int currOp;
    [ReadOnly] public int totalInstigatingOps;

    //... PLAYBACK:
    [ReadOnly] public int currPlaybackStep;
    [ReadOnly] public float currPlaybackTime;
    [ReadOnly] public float currTimeScale = 1f;


    void StartProcessingTurn(List<UnitOp> instigatingOps)
    {
        if (logTurnDebug)
            Debug.LogWarning("Starting to process unit ops.");

        DeselectUnit();

        mode = Mode.ProcessingCommands;
        playbackState = TurnPlaybackState.PLAYING;
        currPlaybackStep = 0;

        currInstigator = selectedUnit;

        totalInstigatingOps = instigatingOps.Count;
		for (int i = 0; i < instigatingOps.Count; i++)
		{
            currInstigatingOps[i] = instigatingOps[i];
		}

        ProcessReactions();
    }


    void ProcessReactions()
    {
        // ... for now this is just taking in the instigating ops
        // ... & putting them directly into turnSteps.

		for (int i = 0; i < totalInstigatingOps; i++)
		{
            allOps[currOp] = currInstigatingOps[i];
            currOp++;

            TurnStep newTurnStep = new TurnStep()
            {
                index = currOp,
                totalOps = 1
            };
            turnSteps[currTurnStep] = newTurnStep;
            currTurnStep++;
		}

        Turn newTurn = new Turn()
        {
            instigator = currInstigator,
            totalSteps = totalInstigatingOps
        };
        turns[currTurn] = newTurn;
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

    public void HandleTurnForward()
	{
		currPlaybackTime += Time.deltaTime * currTimeScale;

        int opIndex = turnSteps[currTurnStep].index;
        int numOpsToTick = turnSteps[currTurnStep].totalOps;
		for (int i = 0; i < numOpsToTick; i++)
		{
            UnitOp op = allOps[i];
            Unit affectedUnit = GameVariables.FetchUnitByIndex(op.unitIndex);
            op.Tick(affectedUnit, currPlaybackTime);
		}

		if (currPlaybackStep >= turns[currTurn].totalSteps)
		{
            Debug.Log("Played through all turnSteps.");
            playbackState = TurnPlaybackState.PAUSED;
            SelectUnit(lastSelectedUnit);
            currInstigator = null;
		}
	}

    
    void ProcessImpacts()
    {

    }

    void ProcessMovement()
    {

    }

}
