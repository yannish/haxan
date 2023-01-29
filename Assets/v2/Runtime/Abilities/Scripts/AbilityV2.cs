using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityTypeV2
{
    MOVEMENT,
    TARGET,
    PASSIVE
}

//... vague thought with StepPhase is that ITB-style, some things resolve in a set order.
public enum StepPhaseV2
{
    UTILITY,
    MOVE,
    ATTACK
}

public class AbilityV2 : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public StepPhaseV2 phase;

    public AbilityTypeV2 type;

    public virtual List<Vector2Int> GetValidMoves(Vector2Int origin, Unit unit) => null;


}
