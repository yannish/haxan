using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnV2 : MonoBehaviour
{
    [ReadOnly] public Unit instigator;

    [ReadOnly] public AbilityV2 ability;

    public Queue<UnitCommand> commands = new();
    public Stack<UnitCommand> commandHistory = new();
    public Stack<UnitCommand> undoneCommands = new();

}
