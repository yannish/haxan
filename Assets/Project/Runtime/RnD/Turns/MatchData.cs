using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchData
{
    public int turnCount;
    public string id;
    public Vector3Int discretePoint;
    public Vector3 point;
    public Quaternion quat;

    //public OpTurn[] turns;
    //public OpTimeBlock[] timeBlocks;
    //public UnitOp[] unitOps;
}
