using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUseButton : MonoBehaviour
	, IPointerEnterHandler
	, IPointerExitHandler
	, IPointerClickHandler
{
	public Image icon;
	public Image background;
	public RectTransform scaleRect;

	[Header("HOVER:")]
	public float hoverFreq;
	public float hoverDamping;
	public Vector2Int hoveredSize;
	public Vector2Int unhoveredSize;
	Vector3 backgroundSizeTarg;
	Vector3 backgroundSizeCurr;
	Vector3 backgroundSizeVel;

	[Header("SELECT:")]
	public float selectFreq;
	public float selectDamp;
	public Vector3 selectedScale;
	public Vector3 deselectedScale;
	Vector3 backgroundScaleTarget;
	Vector3 backgroundScaleCurr;
	Vector3 backgroundScaleVel;

	BoardUI ui;
	[ReadOnly] public Item item;
	[ReadOnly] public ItemUseConfig use;// { get; private set; }

	public Color availableTint;
	public Color unavailableTint;
	[ReadOnly] public bool selected;
	[ReadOnly] public bool hovered;

	public float tweenTime = 1f;
	float tweenTimer;
	Sequence seq;


	public void Init(BoardUI ui, Item item, ItemUseConfig use)
	{
		this.ui = ui;
		this.item = item;
		this.use = use;

		//... TODO: make this overridable for ready-buttons, they have 2 icons:
		this.icon.sprite = use.icon;
		//this.scaleRect = GetComponent<RectTransform>();

		backgroundSizeCurr = unhoveredSize.ToVector3();
		backgroundScaleCurr = deselectedScale;

		SetBackgroundScale();
		SetBackgroundSize();

		//Debug.LogWarning("setting icon: " + use.icon.name);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ui.OnPointerClickItemUseButton(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ui.OnPointerEnterItemUseButton(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ui.OnPointerExitItemUseButton(this);
	}

	public void SetUnavailable()
	{
		icon.color = unavailableTint;
	}

	public void SetAvailable()
	{
		icon.color = availableTint;
	}

	public void Select()
	{
		selected = true;
		UpdateVisuals();
	}

	public void Deselect()
	{
		selected = false;
		UpdateVisuals();
	}

	public void Hover()
	{
		hovered = true;
		UpdateVisuals();
	}

	public void Unhover()
	{
		hovered = false;
		UpdateVisuals();
	}

	void UpdateVisuals()
	{
		UpdateVisualTargets();
		TweenToVisualTargets();
	}

	private void UpdateVisualTargets()
	{
		backgroundSizeTarg = hovered ? hoveredSize.ToVector3() : unhoveredSize.ToVector3();
		backgroundScaleTarget = selected ? selectedScale : deselectedScale;
	}

	private void TweenToVisualTargets()
	{
		seq.Kill();
		seq = DOTween.Sequence();
		seq.SetAutoKill();

		tweenTimer = 0f;
		seq
			.Append(DOTween.To(() => tweenTimer, t => tweenTimer = t, tweenTime, tweenTime))
			.OnUpdate(() =>
			{
				TickVisuals();
				SetBackgroundScale();
				SetBackgroundSize();
			});
	}

	private void TickVisuals()
	{
		Springy.Spring(
			ref backgroundSizeCurr, 
			ref backgroundSizeVel, 
			backgroundSizeTarg, 
			hoverDamping, 
			hoverFreq, 
			Time.deltaTime
			);

		Springy.Spring(
			ref backgroundScaleCurr,
			ref backgroundScaleVel,
			backgroundScaleTarget,
			selectDamp,
			selectFreq,
			Time.deltaTime
			);
	}

	private void SetBackgroundScale() => scaleRect.localScale = backgroundScaleCurr;

	private void SetBackgroundSize() => background.rectTransform.sizeDelta = backgroundSizeCurr;

	internal void Hide()
	{
		backgroundScaleTarget = deselectedScale;
		backgroundScaleCurr = deselectedScale;

		backgroundSizeCurr = unhoveredSize.ToVector3();
		backgroundSizeTarg = unhoveredSize.ToVector3();

		SetBackgroundScale();
		SetBackgroundSize();
	}
}

public static class QuickExtensions
{
	public static Vector3 ToVector3(this Vector2Int vec)
	{
		return new Vector3(vec.x, vec.y, 0f);
	}

	//public static implicit operator Vector3(Vector2Int vec) => new Vector3(vec.x, vec.y, 0f);
}