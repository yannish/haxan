using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Satchel", fileName = "Satchel")]
public class Satchel : ScrObjAbility, ICellCommandDispenser
{
    public CellCommand GetCellCommand(Cell cell)
    {
        throw new System.NotImplementedException();
    }
}
