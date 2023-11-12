using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    MOVEMENT,
    TARGET,
    PASSIVE
}

//... vague thought with StepPhase is that ITB-style, some things resolve in a set order.
public enum StepPhase
{
    UTILITY,
    MOVE,
    ATTACK
}

public class Ability : ScriptableObject
{
    [Header("CONFIG:")]
    public Sprite icon;

    public StepPhase phase;

    public AbilityType type;


    //public virtual void ShowValidMoves(Vector2Int origin, Unit unit) { }

    //public virtual void HideValidMoves() { }


    public virtual void ShowPreview(Vector2Int target, Unit unit) { }

    public virtual void HidePreview() { }

    public virtual List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit) => null;
    // ^^ given unit's position, where can the ability be targeted?

    public virtual List<Vector2Int> GetAffectedCells(Vector2Int origin, Vector2Int targetCoord, Unit unit) => new List<Vector2Int>();

    public virtual PooledMonoBehaviour PreviewAffectedCell(Vector2Int origin, Vector2Int affectedCoord) => null;

    public virtual List<IUnitOperable> FetchUnitOps(Vector2Int targetCoord, Unit unit) => new List<IUnitOperable>();

    public virtual OpInterruptType TryInterruptOp(Unit unit, IUnitOperable op) => OpInterruptType.PASS;

	public virtual bool TryReact(IUnitOperable intigatingOp, out List<IUnitOperable> reaction)
	{
		reaction = null;
		return false;
	}

    //public virtual Queue<UnitCommand> FetchCommandChain_OLD(Vector2Int targetCoord, Unit unit) => new Queue<UnitCommand>();

    //public virtual Queue<UnitCommandStep> FetchCommandStepChain(Vector2Int targetCoord, Unit unit) => new Queue<UnitCommandStep>();

	//public virtual List<UnitCommand> FetchCommandChain(Vector2Int targetCoord, Unit unit) => new List<UnitCommand>();

    

	public Queue<UnitCommandStep> GetSteps(Vector2Int targetCoord, Unit unit)
	{
        if (targetCoord == unit.OffsetPos)
		{
            Debug.LogWarning("move target is same as unit's current position...?", unit.gameObject);
            return null;
		}

		Cell originCell = Board.TryGetCellAtPos(targetCoord);
        if (originCell == null)
            return null;

        Unit foundUnit = Board.GetUnitAtPos(targetCoord);
        if (foundUnit != null && foundUnit.preset != null && !foundUnit.preset.isPassable)
            return null;

        Vector2Int[] path = Board.FindPath_NEW(unit.OffsetPos, targetCoord);
        if (path.Length == 0)
            return null;

        Queue<UnitCommandStep> commandSteps = new Queue<UnitCommandStep>();

        HexDirectionFT toFirstCellDir = unit.OffsetPos.ToNeighbour(path[0]);

        if(unit.Facing != toFirstCellDir)
		{

		}

        return null;
	}
}
