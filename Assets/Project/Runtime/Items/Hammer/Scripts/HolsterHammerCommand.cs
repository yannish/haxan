using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterHammerCommand : UnitCommand
{
    public Hammer hammer;

    public override bool StepsTimeForward() => false;

    public HolsterHammerCommand(
        Unit unit,
        Hammer hammer,
        float duration
        )
    {
        this.unit = unit;
        this.hammer = hammer;
        this.duration = duration;
    }

    public override void OnBeginTick()
    {
        hammer.SetDrawAnim();
        hammer.TickReadyAnim(1f);
    }

    public override bool Tick_OLD(float timeScale = 1)
    {
        base.Tick_OLD(timeScale);

        hammer.TickReadyAnim(1f - currProgress);

        return CheckComplete(timeScale);
    }

    public override void OnBeginReverseTick()
    {
        hammer.SetReadyAnim();
        hammer.TickReadyAnim(0f);
    }
}
