using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class RectUIElement : UIElement
{
	void Awake()
	{
		if (clickRect != null)
		{
			cachedAnchoredPos = clickRect.anchoredPosition;
		}

		if(hoverRect != null)
		{
			cachedDimensions = new Rect(0f, 0f, hoverRect.rect.width, hoverRect.rect.height);
		}
	}


	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		HoverTweenToTarget(hoveredThickness);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		HoverTweenToTarget(0f);
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		ClickTween(-clickDepth);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		ClickTween(0f);
	}



	[Header("CLICK:")]
	public RectTransform clickRect;
	public FloatSpring clickSpring;

	public float clickDepth = 10f;
	public float clickVelocity = 200f;
	public float clickDuration = 1f;

	[ReadOnly] public Vector2 cachedAnchoredPos;

	Sequence clickSequence;
	private void ClickTween(float newClickTarget)
	{
		if (clickRect == null)
			return;

		if (clickSequence.IsActive())
			clickSequence.Kill();

		clickSequence = DOTween.Sequence();

		clickSequence.SetAutoKill();

		clickSpring.currTarget = newClickTarget;
		//clickSpring.velocity = -clickVelocity;

		clickSequence
			.AppendInterval(clickDuration)
			.OnUpdate(() =>
			{
				clickSpring.Step();

				//Springy.Spring(
				//	ref clickSpring.currValue,
				//	ref clickSpring.velocity,
				//	clickSpring.currTarget,
				//	clickSpring.damp,
				//	clickSpring.freq,
				//	Time.deltaTime
				//	);

				clickRect.anchoredPosition = cachedAnchoredPos + new Vector2(0f, clickSpring.currValue);
			});
	}


	[Header("HOVER:")]
	public RectTransform hoverRect;
	public float hoverTweenDuration = 0.5f;
	public float hoveredThickness = 50f;

	public FloatSpring hoverSpring;

	[ReadOnly] public Rect cachedDimensions;
	[ReadOnly] public float currThickness;

	Sequence hoverSeq;

	private void HoverTweenToTarget(float newTarget)
	{
		if (hoverRect == null)
			return;

		hoverSpring.currTarget = newTarget;

		if (hoverSeq.IsActive())
			hoverSeq.Kill();

		hoverSeq = DOTween.Sequence();
		hoverSeq.SetAutoKill();
		hoverSeq
			.AppendInterval(hoverTweenDuration)
			.OnUpdate(() =>
			{
				hoverSpring.Step();

				hoverRect.SetSizeWithCurrentAnchors(
					RectTransform.Axis.Horizontal,
					cachedDimensions.width + hoverSpring.currValue
					);

				hoverRect.SetSizeWithCurrentAnchors(
					RectTransform.Axis.Vertical,
					cachedDimensions.height + hoverSpring.currValue
					);
			});
	}
}
