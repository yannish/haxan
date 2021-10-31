using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnProcessor
{
    void ProcessTurns();

    void SetPhase(TeamPhase teamPhase);

    void RecordTurn(Turn turn);

    void Undo();

    void ProcessEnemyTurns();

    bool IsProcessing { get; }
}
