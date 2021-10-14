using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColorShifter : MonoBehaviour
{
    public ColorLensManager lensManager;

    [ReadOnly] public Color currColor;

    public float easeTime;

    public Ease ease;

    public UnityEvent<Color> onUpdate;

	private void Awake()
	{
        if (lensManager == null)
            return;

        lensManager.onTargetChanged += TweenToTarget;
	}

	Sequence seq;
	private void TweenToTarget(Color obj)
	{
		if (seq.IsActive())
			seq.Kill();

		seq = DOTween.Sequence();
		seq.SetAutoKill();
		seq.Append(DOTween.To(() => currColor, x => currColor = x, obj, easeTime))
			.OnUpdate(() => onUpdate?.Invoke(currColor))
			.SetEase(ease);
		seq.OnKill(() => seq = null);
	}
}
