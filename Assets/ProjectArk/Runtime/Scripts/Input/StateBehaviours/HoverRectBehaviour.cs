using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverRectBehaviour : RectBehaviour
{

	[Header("HOVER")]
	public float hoveredThickness = 50f;
	[ReadOnly] public float currThickness;


	//public override void Enter()
	//{
	//	//base.Enter();
	//	//HoverTweenToTarget(hoveredThickness);
	//}

	//public override void Exit()
	//{
	//	//base.Exit();
	//	//HoverTweenToTarget(0f);
	//}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		rectTransform.SetSizeWithCurrentAnchors(
			RectTransform.Axis.Horizontal,
			cachedDimensions.width + currVector.x
			);

		rectTransform.SetSizeWithCurrentAnchors(
			RectTransform.Axis.Vertical,
			cachedDimensions.height + currVector.x
			);
	}

	//private void HoverTweenToTarget(float newTarget)
	//{
	//	if (seq.IsActive())
	//		seq.Kill();

	//	seq = DOTween.Sequence();
	//	seq.SetAutoKill();
	//	seq.Append(DOTween.To(() => currThickness, x => currThickness = x, newTarget, duration))
	//		.OnUpdate(() =>
	//		{
	//			rectTransform.SetSizeWithCurrentAnchors(
	//				RectTransform.Axis.Horizontal,
	//				cachedDimensions.width + currThickness
	//				);

	//			rectTransform.SetSizeWithCurrentAnchors(
	//				RectTransform.Axis.Vertical,
	//				cachedDimensions.height + currThickness
	//				);
	//		})
	//		.SetEase(ease);
	//	seq.OnKill(() => seq = null);
	//}
}
