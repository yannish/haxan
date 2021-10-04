using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorFlowController : CellFlowController
{
	public override void HoverPeek() { fsm?.SetTrigger("clickable"); }
	public override void HoverUnpeek() { fsm?.SetTrigger("unclickable"); }

    //public override void HoverPeek() { animator?.SetBool("clickable", true); }
    //public override void HoverUnpeek() { animator?.SetBool("clickable", false); }
}