using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Machete", fileName = "Machete")]
public class Machete : Item
{
	public PooledMonoBehaviour slashPfx;

    public override UnitCommand RespondToCommand(Unit unit, UnitCommand command)
    {
        if (command is StepCommandV2)
        {
            StepCommandV2 stepCommand = command as StepCommandV2;

			///... get all neighbouring coords of from & to:

			//MacheteSlashCommand slashCommand = new MacheteSlashCommand(unit, stepCommand)
        }

        return null;
    }
}

public class MacheteSlashCommand : UnitCommand
{
    public Vector2Int fromCoord;
    public Vector2Int toCoord;

    public MacheteSlashCommand(Unit unit, Vector2Int fromCoord, Vector2Int toCoord, float duration)
	{
        this.unit = unit;
        this.toCoord = toCoord;
        this.fromCoord = fromCoord;
        this.duration = duration;
    }

	public override void OnBeginTick()
	{
		//... start some particles / visuals
	}

    public override void OnBeginReverseTick()
    {
        //... being reversing some particles / visuals
    }

	public override void OnCompleteTick()
	{
		//... final stepping of particles / visuals
	}

	public override void OnCompleteReverseTick()
	{
		//... final un-stepping of particles / visuals
	}

	public override void Execute()
	{
		//... put out actual damage
	}

	public override void Undo()
	{
		//... undo any actual damage
	}
}
