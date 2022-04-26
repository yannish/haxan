using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * some of these names seem odd.
 * RecordTurn is just taking in a turn and starting to process
 * ... doesn't put them in a record.
 */

public interface ITurnProcessor
{
    void ProcessTurns();

    void SetPhase(TeamPhase teamPhase);

    TeamPhase CurrPhase { get; }

    //...   soon as you record a turn, processor will return IsProcessing,
    //      until the turn is finished being executed.
    void RecordTurn(Turn turn);

    void Undo();

    //void ProcessEnemyTurns();

    bool IsProcessing { get; }
}
