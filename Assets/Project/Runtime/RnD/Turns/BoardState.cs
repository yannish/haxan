using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

	public Turn[] turns = new Turn[MAX_TURNS];
	public TurnStep[] turnSteps = new TurnStep[MAX_TURN_STEPS];
	public IUnitOperable[] allOps = new IUnitOperable[MAX_OPS];

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
