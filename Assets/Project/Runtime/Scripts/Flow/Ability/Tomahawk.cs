using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Tomahawk", fileName = "Tomahawk")]
public class Tomahawk : ScrObjAbility, ICellCommandDispenser
{
    public CellCommand GetCellCommand(Cell cell)
    {
        throw new System.NotImplementedException();
    }
}
