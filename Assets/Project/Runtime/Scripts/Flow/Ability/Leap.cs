using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Leap", fileName = "Leap")]
public class Leap : ScrObjAbility
    //, ICellCommandDispenser
{
    public CellCommand GetCellCommand(Cell cell)
    {
        throw new System.NotImplementedException();
    }
}
