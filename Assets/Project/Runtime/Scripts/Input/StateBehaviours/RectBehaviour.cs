using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectBehaviour : QuickStateBehaviour
{
	public RectTransform rectTransform;


	[Header("SPRING:")]
	public float duration = 0.5f;
	public float damping;
	public float frequency;

	public Vector3 enterTarget;

	[ReadOnly] public Vector3 currVector;
	[ReadOnly] public Vector3 targetVector;
	[ReadOnly] public Vector3 currVectorVelocity;

	[ReadOnly] public Vector3 cachedLocalPos;
	[ReadOnly] public Vector3 cachedAnchorPos;
	[ReadOnly] public Rect cachedDimensions;

	protected Sequence seq;

	protected virtual void Awake()
	{
		rectTransform = GetComponentInParent<RectTransform>();
		if (rectTransform == null)
			return;

		cachedLocalPos = rectTransform.localPosition;

		cachedAnchorPos = rectTransform.anchoredPosition;

		cachedDimensions = new Rect(
			0f, 
			0f, 
			rectTransform.rect.width, 
			rectTransform.rect.height
			);
	}

	public override void Enter()
	{
		base.Enter();
		SetTween(enterTarget);
		//DipTweenToTarget(dipDepth);
	}

	public override void Exit()
	{
		base.Exit();
		SetTween(Vector3.zero);
		//DipTweenToTarget(0f);
	}


	protected virtual void SetTween(Vector3 newTarget)
	{
		if (rectTransform == null)
			return;

		if (seq.IsActive())
			seq.Kill();

		seq = DOTween.Sequence();

		targetVector = newTarget;

		OnTweenSet();

		seq
			.AppendInterval(duration)
			.OnUpdate(() =>
		   {
			   Springy.Spring(
				   ref currVector, 
				   ref currVectorVelocity, 
				   targetVector, 
				   damping, 
				   frequency, 
				   Time.deltaTime
				   );
			   OnUpdate();
		   });
	}

	protected virtual void OnTweenSet() { }

	protected virtual void OnUpdate() { }
}
