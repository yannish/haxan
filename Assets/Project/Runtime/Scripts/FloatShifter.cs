using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloatShifter : MonoBehaviour
{
    public FloatLensManager lensManager;

	[ReadOnly] public float currValue;

	public float easeTime;

	public Ease ease;

	public UnityEvent<float> onUpdate;

	public void Awake()
	{
		if (lensManager == null)
			return;

		lensManager.onTargetChanged += TweenToTarget;
	}


	Sequence seq;
	private void TweenToTarget(float newTarget)
	{
		if(seq.IsActive())
			seq.Kill();

		seq = DOTween.Sequence();
		seq.SetAutoKill();
		seq.Append(DOTween.To(() => currValue, x => currValue = x, newTarget, easeTime))
			.OnUpdate(() => onUpdate?.Invoke(currValue))
			.SetEase(ease);
		seq.OnKill(() => seq = null);
	}
}
