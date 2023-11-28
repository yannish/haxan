using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TurnHistory
{

}

[System.Serializable]
public class BoardState
{
    public List<UnitState> unitStates = new List<UnitState>();
}

[System.Serializable]
public class BoardHistory
{
	public const int MAX_OPS = 100;
	public const int MAX_INSTIGATING_OPS = 100;
	public const int MAX_TURNS = 100;
	public const int MAX_TURN_STEPS = 100;

	[ReadOnly] public int turnCount = 0;
	[ReadOnly] public int totalCreatedTurnSteps = 0;
	[ReadOnly] public int totalCreatedOps = 0;
	//public int turnStepCount;



	public Turn[] turns = new Turn[MAX_TURNS];
	public TurnStep[] turnSteps = new TurnStep[MAX_TURN_STEPS];
	public UnitOp[] allOps = new UnitOp[MAX_OPS];
	public IUnitOperable[] allOps_OLD = new IUnitOperable[MAX_OPS];
	public UnitOp_STRUCT[] allOps_NEW = new UnitOp_STRUCT[MAX_OPS];
}

[System.Serializable]
public struct UnitState
{
    public string templatePath;
    public UnitType type;
    public Vector2Int offsetPos;
    public HexDirectionFT facing;

    //public int id;
    //public Vector3 pos;
    //public Quaternion rot;
}
