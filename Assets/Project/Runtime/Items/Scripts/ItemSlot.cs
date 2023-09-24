using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
	, IPointerEnterHandler
	, IPointerExitHandler
	, IPointerClickHandler
{
	BoardUI ui;
	public Item item;
	ItemConfig itemConfig;

	public Image icon;
	public Image background;
	public ItemUseButton[] useButtons;//= new List<ItemUseButton>();
	public ItemUseButton readyButton;
	public TextMeshProUGUI itemName;
	//public ItemUseButton readyUseButton;
	//public Image readyIcon;
	//public Image unreadyIcon;

	public Color deselectedColor;
	public Color selectedColor;
	public Color hoveredColor;


	[Header("USES:")]
	public RectTransform usesRect;
	public HorizontalLayoutGroup useLayoutGroup;

	[Header("ANIMATION:")]
	public bool animateLayoutGroup;
	public float closedPadding = -100f;
	public float currPadding;
	public float currTargetPadding;
	public float currPaddingVelocity;
	public float paddingSmoothTime;
	public float paddingMaxSpeed;

	public float closedSpacing = -100f;
	public float currSpacing;
	public float currTargetSpacing;
	public float currSpacingVelocity;
	public float spacingSmoothTime;
	public float spacingMaxSpeed;

	[Header("DEBUG:")]
	public bool debugInput;

	bool hovered;
	bool selected;


	void Start()
	{
		//closedPadding = -60f;
		//closedSpacing = -60f;
		if (!animateLayoutGroup)
			return;

		currPadding = closedPadding;
		currSpacing = closedSpacing;

		UpdateLayoutGroup();

		//cachedPadding = useLayoutGroup.padding.left;
		//cachedSpacing = useLayoutGroup.spacing;c
	}

	public void Init(BoardUI ui, Item item)
	{
		this.ui = ui;
		this.item = item;
		this.icon.sprite = item.icon;
		this.itemName.SetText(item.name.ToUpper());

		Debug.LogWarning("setting text: " + item.name.ToUpper());

		//if (item.config.requiresReadying)
		//{
		//	readyButton.gameObject.SetActive(true);
		//	readyButton.Init(ui, item);
		//}
		//else
		//{
		//	readyButton.gameObject.SetActive(false);
		//}

		useButtons = GetComponentsInChildren<ItemUseButton>();
		foreach (var button in useButtons)
		{
			button.gameObject.SetActive(false);
		}

		for (int i = 0; i < item.allUses.Count && i < useButtons.Length; i++)
		{
			ItemUseButton button = useButtons[i];
			button.gameObject.SetActive(true);

			Debug.LogWarning("slotting: " + i);

			ItemUseConfig itemUse = item.allUses[i];
			button.Init(ui, item, itemUse);

			itemUse.UpdateButton(button, item);

			//if (itemUse.IsReadyUse)
			//{
			//	readyButton = button;
			//}

			//if (item.config.requiresReadying)
			//{
			//	if (item.isReadied && !itemUse.IsReadyUse)
			//		button.SetAvailable();
			//	else
			//		button.SetUnavailable();
			//}
		}
	}

	void SlotUseButton(ItemUseConfig useConfig)
	{
		
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ui.OnPointerClickedItemButton(item, this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ui.OnPointerEnteredItemSlot(item, this);
		hovered = false;

		currTargetPadding = 0f;
		currTargetSpacing = 0f;

		UpdateVisualTargets();

		if (debugInput)
			Debug.LogWarning("pointer entered:" + item.name, this.gameObject);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ui.OnPointerExitedItemSlot(item, this);
		hovered = true;

		currTargetPadding = closedPadding;
		currTargetSpacing = closedSpacing;

		UpdateVisualTargets();

		if (debugInput)
			Debug.LogWarning("pointer exited:" + item.name, this.gameObject);
	}

	public void Select()
	{
		selected = true;
		UpdateVisualTargets();
	}

	public void Deselect()
	{
		selected = false;
		UpdateVisualTargets();
	}

	public void Hide()
	{
		selected = false;
		hovered = false;
		foreach(var button in useButtons)
		{
			button.Hide();
		}
		//UpdateVisualTargets();
		//UpdateLayoutGroup();
	}

	public float tweenTime;
	public float tweenTimer;
	Sequence seq;


	void UpdateVisualTargets()
	{
		if (!animateLayoutGroup)
			return;

		//Debug.LogWarning("UPDATIN VISUAL");

		if (selected)
		{
			background.color = selectedColor;
			return;
		}

		if (hovered)
		{
			background.color = hoveredColor;
			return;
		}

		background.color = deselectedColor;
	}

	void TweenToVisualTargets()
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
				UpdateLayoutGroup();
			});
	}


	void TickVisuals()
	{
		currPadding = Mathf.SmoothDamp(
			currPadding,
			currTargetPadding,
			ref currPaddingVelocity,
			paddingSmoothTime,
			paddingMaxSpeed,
			Time.deltaTime
			);

		currSpacing = Mathf.SmoothDamp(
			currSpacing,
			currTargetSpacing,
			ref currSpacingVelocity,
			spacingSmoothTime,
			spacingMaxSpeed,
			Time.deltaTime
			);
	}

	void UpdateLayoutGroup()
	{
		useLayoutGroup.spacing = currSpacing;
		useLayoutGroup.padding.left = (int)currPadding;
	}
}
