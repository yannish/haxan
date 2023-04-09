using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BashCommand : UnitCommand
/*
 * simple bash that pushes one unit in one direction.
 * more elaborate versions could push out several units radially, or several in a line, etc.
 * 
 * ... what about "sliding" back. that's sort fo a different command, akin to Step.
 */
{
    public Unit dispenserUnit;
    public Unit receiverUnit;
    //public List<Unit> bashees = new List<Unit>();

    public Vector2Int dispenserCoord;
    public Vector2Int receiverCoord;
    public Vector2Int destinationCoord;

    HexDirectionFT dir;
    int distance;

    public BashCommand(Unit dispenser, Unit receiver, int distance)
        //^^ distance should be calculated ahead of time by the situation generating the bash?
	{
        this.dispenserUnit = dispenser;
        this.receiverUnit = receiver;
        this.distance = distance;

        dispenserCoord = Board.WorldToOffset(dispenser.transform.position);
        receiverCoord = Board.WorldToOffset(receiver.transform.position);

        dir = dispenserCoord.ToNeighbour(receiverCoord);

        destinationCoord = receiverCoord.Step(dir, 1);

		//destinationCoord = receiverCoord.neigh
	}

	public override void OnBeginTick()
	{
        //      if(distance == 0)
        //{
        //          ... do some specific non-moving bash stuff, if that's a thing.
        //return;
        //}

        Board.OnUnitExitedCell(receiverUnit, receiverCoord);
        Board.RespondToCommandBeginTick(receiverUnit, this);
	}

	public override void OnCompleteTick()
	{
		//Board.OnUnitEnteredCell(receiverUnit, )
	}
}
