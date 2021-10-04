using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : Task
{
    public float waitTime = 0f;
    public string msg = "";

    public override void Exit()
    {
        if (msg.Length > 0)
            Debug.Log(msg);

        base.Exit();
    }

    public override NodeStatus Tick()
    {
        base.Tick();

        if (elapsedTime > waitTime)
            FinishSuccesfully();

        return status;
    }
}
