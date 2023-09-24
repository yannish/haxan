using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Hammer : Item
{
	public ReadyHammerUse readyUse;
	public SwingHammerUse swingUse;

	//public ScrubClip readyClip;
	//public ScrubClip swingClip;

	//int readyHash;
	//int swingHash;

	private void Start()
	{
		allUses.Add(readyUse);
		allUses.Add(swingUse);

		//readyClip = clipHandler.RegisterClip(readyUse.clip);
		//swingClip = clipHandler.RegisterClip(swingUse.clip);
	}


	public void SetReadyAnim() => clipHandler.SetClip(readyUse.readyClip);

	public void TickReadyAnim(float progress)
	{
		clipHandler.Scrub(progress);
	}

	public void SetSwingClockwiseAnim() => clipHandler.SetClip(swingUse.cwSwingClip);

	public void SetSwingCounterclockwiseAnim() => clipHandler.SetClip(swingUse.ccwSwingClip);

	public void TickSwingAnim(float progress)
	{
		clipHandler.Scrub(progress);
	}

	public void SetDrawAnim() => clipHandler.SetClip(readyUse.drawClip);

	public void TickDrawAnim(float progress)
	{
		clipHandler.Scrub(progress);
	}
}
