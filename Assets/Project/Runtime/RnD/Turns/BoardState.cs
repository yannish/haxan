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
	public BoardHistory history;
}

[System.Serializable]
public class BoardHistory
{
	public const int MAX_OPS = 100;
	public const int MAX_INSTIGATING_OPS = 100;
	public const int MAX_TURNS = 100;
	public const int MAX_TURN_STEPS = 100;

	public int turnCount = 0;
	public int turnStepCount = 0;
	public int opCount = 0;

	public Turn[] turns = new Turn[MAX_TURNS];
	public TurnStep[] turnSteps = new TurnStep[MAX_TURN_STEPS];
	public UnitOp[] allOps = new UnitOp[MAX_OPS];
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
