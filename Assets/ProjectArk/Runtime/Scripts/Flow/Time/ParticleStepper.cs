using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStepper : TurnStepper
{
	public ParticleSystem pfx;

	public override float duration { get { return pfx ? pfx.main.duration : 0f; } }
	public override float stepSize { get { return pfx && steps != 0 ? pfx.main.duration / steps : 0f; } }

	private Sequence seq;

	FlowController controller;

	protected override void Awake()
	{
		base.Awake();
		pfx = GetComponentInChildren<ParticleSystem>();
		//pfx.useAutoRandomSeed = true;
		//Reseed();
	}

	private void Reseed() { pfx.randomSeed = (uint)Random.Range(0, 100000); }

	protected override void Tick() { pfx.Simulate(currTime, true, true, false); }

	protected override void Kill()
	{
		base.Kill();

		currTime = 0f;
		velocity = 0f;
		currStep = 0;

		gameObject.SetActive(false);
	}

	private void ScrubTo(bool kill = false)
	{
		seq = DOTween.Sequence();

		seq
			.Append(DOTween.To(() => currTime, x => currTime = x, targetTime, easeTime))
			.OnUpdate(() => pfx.Simulate(currTime, true, true, false))
			.SetEase(Ease.OutQuint);

		if (kill)
			seq.OnComplete(() => Kill());
	}
}