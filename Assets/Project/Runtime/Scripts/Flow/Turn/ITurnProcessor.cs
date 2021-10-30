using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnProcessor
{
    void RecordTurn(Turn turn);

    void Undo();

    void ProcessEnemyTurns();

    bool IsProcessing { get; }
}
