using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickRectBehaviour : RectBehaviour
{
	//[Header("CLICK")]
	//public float dipDepth = 10f;
	//[ReadOnly] public float cachedDipHeight;

	//protected override void Awake()
	//{
	//	base.Awake();
	//	//currVector = rectTransform.localPosition;
	//	cachedDipHeight = rectTransform.localPosition.y;
	//}

	protected override void OnTweenSet()
	{

	}

	protected override void OnUpdate()
	{
		rectTransform.anchoredPosition = cachedAnchorPos + currVector;
		//rectTransform.localPosition = cachedLocalPos + currVector;
	}

	public override void Enter()
	{
		base.Enter();
		Debug.LogWarning("ENTERED CLICK RECT");
		//DipTweenToTarget(dipDepth);
	}

	public override void Exit()
	{
		base.Exit();
		Debug.LogWarning("Exited CLICK RECT");
		//DipTweenToTarget(0f);
	}

	//[ReadOnly] public float currDipHeight;

	//private void DipTweenToTarget(float newDipTarget)
	//{
	//	if (rectTransform == null)
	//		return;

	//	if (seq.IsActive())
	//		seq.Kill();

	//	seq = DOTween.Sequence();
	//	seq.SetAutoKill();

	//	//seq.Append(DOTween.To(() => currDipHeight, x => currDipHeight = x, newDipTarget, duration))
	//	//	.OnUpdate(() =>
	//	//	{
	//	//		//Springy.Spring(curr)
	//	//		rectTransform.localPosition = rectTransform.localPosition.With(
	//	//			y: cachedDipHeight + currDipHeight
	//	//			);
	//	//	})
	//	//	//.SetEase(ease)
	//	//	.SetLoops(2, LoopType.Yoyo);

	//	seq.OnKill(() => seq = null);
	//}


}
