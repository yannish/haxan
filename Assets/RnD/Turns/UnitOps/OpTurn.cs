using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OpTurn
{
    public Unit instigator; //... have to find a way to serialize & then recreate this on the fly.

    public int startTimeBlockIndex;
    public int timeBlockCount;
}

[System.Serializable]
public class OpTimeBlock
{
    int opStartIndex;
    int opCount;
}
