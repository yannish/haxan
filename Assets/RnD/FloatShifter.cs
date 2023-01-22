using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum FloatShiftStyle
{
	TWEEN,
	SMOOTH
}

public class FloatShifter : MonoBehaviour
{
    public FloatLensManager lensManager;
	public FloatShiftStyle style;


	[Header("Tween")]
	[ReadOnly] public float currTweenValue;
	public float easeTime;
	public Ease ease;


	[Header("Smooth Damp")]
	public float smoothTime = 1f;
	public float smoothDuration = 2f;
	public float maxSpeed = -1f;
	[ReadOnly]
	public float currSmoothedValue;
	[ReadOnly]
	public float currSmoothedVelocity;
	[ReadOnly]
	public float currSmoothedTarget;
	[ReadOnly]
	public float currSmoothTimer;


	[Header("Drive")]
	public UnityEvent<float> onUpdate;

	public void Awake()
	{
		if (lensManager == null)
			return;

		currSmoothedValue = lensManager.baseValue;

		switch (style)
		{
			case FloatShiftStyle.TWEEN:
				lensManager.onTargetChanged += TweenToTarget;
				break;

			case FloatShiftStyle.SMOOTH:
				lensManager.onTargetChanged += SmoothDampToTarget;
				break;
		}
	}


	Sequence seq;
	private void TweenToTarget(float newTarget, float inTime)
	{
		if(seq.IsActive())
			seq.Kill();

		seq = DOTween.Sequence();
		seq.SetAutoKill();

		seq.Append(DOTween.To(() => currTweenValue, x => currTweenValue = x, newTarget, easeTime))
			.OnUpdate(() => onUpdate?.Invoke(currTweenValue))
			//.SetAutoKill()
			.SetEase(ease);

		seq.OnKill(() => seq = null);
	}

	private void SmoothDampToTarget(float newTarget, float inTime)
	{
		if (seq.IsActive())
			seq.Kill();

		seq = DOTween.Sequence();
		seq.SetAutoKill();

		currSmoothedTarget = newTarget;
		currSmoothTimer = 0f;

		seq.Append(DOTween.To(() => currSmoothTimer, x => currSmoothTimer = x, 1f, smoothDuration))
			.OnUpdate(() =>
			{
				currSmoothedValue = Mathf.SmoothDamp(
					currSmoothedValue, 
					currSmoothedTarget, 
					ref currSmoothedVelocity, 
					smoothTime,
					maxSpeed > 0f ? maxSpeed : Mathf.Infinity,
					Time.deltaTime
					);
				onUpdate?.Invoke(currSmoothedValue);
			})
			.SetEase(Ease.Linear);

		seq.OnKill(() => seq = null);
	}
}
