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
    public string templatePath;
    public UnitType type;
    public Vector2Int offsetPos;
    public HexDirectionFT facing;
    //public int id;
    //public Vector3 pos;
    //public Quaternion rot;
}