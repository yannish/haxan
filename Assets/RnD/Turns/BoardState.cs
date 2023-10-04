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
    public List<GroundMoveOp> ops = new List<GroundMoveOp>();
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

public struct DummyOpA : IUnitOperable
{
	public Vector2Int fromCoord;
	public Vector2Int toCoord;

	public void DrawInspectorContent()
	{
		throw new System.NotImplementedException();
	}

	public void Execute(Unit unit)
	{
		throw new System.NotImplementedException();
	}

	public void Tick(Unit unit, float t)
	{
		throw new System.NotImplementedException();
	}

	public void Undo(Unit unit)
	{
		throw new System.NotImplementedException();
	}
}

public struct DummyOpB : IUnitOperable
{
	public HexDirectionFT fromDir;
	public HexDirectionFT toDir;

	public void DrawInspectorContent()
	{
		throw new System.NotImplementedException();
	}

	public void Execute(Unit unit)
	{
		throw new System.NotImplementedException();
	}

	public void Tick(Unit unit, float t)
	{
		throw new System.NotImplementedException();
	}

	public void Undo(Unit unit)
	{
		throw new System.NotImplementedException();
	}
}


