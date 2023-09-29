using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BoardState
{
    public List<UnitState> unitStates = new List<UnitState>();
}

[System.Serializable]
public struct UnitState
{
    public string name;
    public int id;
    public Vector2Int offsetPos;
    public HexDirectionFT facing;
    //public Vector3 pos;
    //public Quaternion rot;
}