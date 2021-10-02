using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//... doing this because a cell might give you a flow, or an icon on the HUD (ex. an ability or item)
public interface IFlowable
{
	FlowController Flow { get; }
}
