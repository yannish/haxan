using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CellStateV2
{
    HOVERED = 1 << 1,
    CLICKABLE = 1 << 2,
    SELECTED = 1 << 3,
    PATHHINTED = 1 << 4
}

[Serializable]
public struct HexRingSize
{
    public float size;
    public float thickness;
    public static implicit operator Vector2(HexRingSize cellHexState)
    {
        return new Vector2(cellHexState.size, cellHexState.thickness);
    }
    public static implicit operator Vector3(HexRingSize cellHexState)
    {
        return new Vector3(cellHexState.size, cellHexState.thickness, 0f);
    }
}


public class PooledCellVisuals : PooledMonoBehaviour
{
    public override void Play(int playParams)
    {
        //      CellStateV2 state = (CellStateV2)playParams;
        //switch (state)
        //{
        //	case CellStateV2.HOVERED:
        //              Debug.LogWarning("HOVERING CELL:");
        //              SetTrigger(CellState.hover);
        //              break;
        //	case CellStateV2.CLICKABLE:
        //              SetTrigger(CellState.clickable);
        //              break;
        //	case CellStateV2.SELECTED:
        //              SetTrigger(CellState.select);
        //              break;
        //	default:
        //		break;
        //}
    }

    public override void Stop(int stopParams)
    {
        //CellStateV2 state = (CellStateV2)stopParams;
        //switch (state)
        //{
        //    case CellStateV2.HOVERED:
        //        UnsetTrigger(CellState.hover);
        //        break;
        //    case CellStateV2.CLICKABLE:
        //        UnsetTrigger(CellState.clickable);
        //        break;
        //    case CellStateV2.SELECTED:
        //        UnsetTrigger(CellState.select);
        //        break;
        //    default:
        //        break;
        //}

        //UnsetTrigger(CellState.hover);
    }

    public override void Tick()
	{
        SmoothChange();
        base.Tick();
	}

    public override bool IsProcessing()
    {
        bool result = currSmoothTime > 0f || hovered || selected || clickable || pathHinted;
        return result;
    }

    //public Action onReturnToPool;
    //public void Return() 
    //{
    //    onReturnToPool?.Invoke();
    //    onReturnToPool = null;
    //}


    public bool logDebug;

    [Header("INTERACTION: ")]
    public MaterialBlockHandle hoverBlock;
    public MaterialBlockFloat interactionHexSize;
    public MaterialBlockFloat interactionHexThickness;
    public MaterialBlockColor interactionColor;

    public Color interactionBaseColor;

    [ReadOnly] public Vector3 currInteractionSize;
    [ReadOnly] public Vector3 currInteractionTargetSize;
    [ReadOnly] public Vector3 currInteractionSizeVelocity;

    [ReadOnly] public Color currHoverTargetColor;
    [ReadOnly] public Color currHoverColor;
    [ReadOnly] public float currHoverAlpha;
    [ReadOnly] public float currHoverAlphaVel;
    [ReadOnly] public float currHoverAlphaTarget;
    public float hoverAlphaSmoothTimeUp;
    public float hoverAlphaSmoothTimeDown;


    //[ReadOnly] 
    public bool hovered;
    public EditorButton hoverOnBtn = new EditorButton("HoverOn", true);
    void HoverOn()
    {
        SetTrigger(CellState.hover);
        UpdateVisualTargets();
    }

    public EditorButton hoverOffBtn = new EditorButton("HoverOff", true);
    void HoverOff()
    {
        UnsetTrigger(CellState.hover);
        UpdateVisualTargets();
    }

    public float hoveredAlpha = 0.5f;
    public HexRingSize hoveredHexRingSize;
    public HexRingSize unhoveredHexRingSize;



    [Header("CLICKABLE:")]
    public EditorButton clickableOnBtn = new EditorButton("ClickableOn", true);
    void ClickableOn()
    {
        SetTrigger(CellState.clickable);
        UpdateVisualTargets();
    }

    public EditorButton clickableOffBtn = new EditorButton("ClickableOff", true);
    void ClickableOff()
    {
        UnsetTrigger(CellState.clickable);
        UpdateVisualTargets();
    }

    //[ReadOnly] 
    public bool clickable;

    public HexRingSize clickableSize;


    [Header("SELECT:")]
    //[ReadOnly] 
    public bool selected;
    public EditorButton selectOnBtn = new EditorButton("SelectOn", true);
    void SelectOn()
    {
        SetTrigger(CellState.select);
        UpdateVisualTargets();
    }

    public EditorButton selectOffBtn = new EditorButton("SelectOff", true);
    void SelectOff()
    {
        UnsetTrigger(CellState.select);
        UpdateVisualTargets();
    }
    public HexRingSize selectedSize;


    [Header("PATH:")]
    public MaterialBlockHandle pathBlock;
    public MaterialBlockFloat pathBlockSize;
    public MaterialBlockFloat pathBlockThickness;
    public MaterialBlockColor pathBlockColor;
    public Color pathableColor;

    //[ReadOnly] 
    public bool pathHinted;
    public EditorButton pathHintOnBtn = new EditorButton("PathHintOn", true);
    void PathHintOn()
    {
        SetTrigger(CellState.pathHint);
        UpdateVisualTargets();
    }

    public EditorButton pathHintOffBtn = new EditorButton("PathHintOff", true);
    void PathHintOff()
    {
        UnsetTrigger(CellState.pathHint);
        UpdateVisualTargets();
    }

    //[ReadOnly] 
    public bool pathShown;
    public EditorButton pathShownOnBtn = new EditorButton("PathShownOn", true);
    void PathShownOn()
    {
        SetTrigger(CellState.pathShown);
        UpdateVisualTargets();
    }

    public EditorButton pathShownOffBtn = new EditorButton("PathShownOff", true);
    void PathShownOff()
    {
        UnsetTrigger(CellState.pathShown);
        UpdateVisualTargets();
    }

    public HexRingSize pathShownSize;
    public HexRingSize pathHintSize;
    public HexRingSize pathHiddenSize;

    [ReadOnly] public Vector3 pathCurrSize;
    [ReadOnly] public Vector3 pathTargetSize;
    [ReadOnly] private Vector3 pathSizeVelocity;

    [ReadOnly] public Color pathCurrColor;
    [ReadOnly] public Color currPathTargetColor;



    [Header("SMOOTHING:")]
    public float smoothTime;
    public float colorLerpTime = 1f;

    public FloatReference interactionFrequency;
    public FloatReference interactionFrequencyRange;

    public FloatReference interactionDamping;
    public FloatReference interactionDampingRange;

    public FloatReference pathFrequency;
    public FloatReference pathFrequencyRange;

    public FloatReference pathDamping;
    public FloatReference pathDampingRange;


    [Header("THREAT:")]
    public Color threatColor;


	protected override void Awake()
	{
		currInteractionSize = hoveredHexRingSize;
        UpdateMaterialBlock();
    }

    public void Hover()
    {
        hovered = true;
        UpdateVisualTargets();
    }

    public void Unhover()
    {
        hovered = false;
        UpdateVisualTargets();
    }

    public void Clickable()
	{
        clickable = true;
        UpdateVisualTargets();
    }

    public void Unclickable()
	{
        clickable = false;
        UpdateVisualTargets();
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

    public void HintPath()
	{
        pathHinted = true;
        UpdateVisualTargets();
	}

    public void UnhintPath()
	{
        pathHinted = false;
        UpdateVisualTargets();
	}

    public void SetTrigger(CellState trigger) => SetTriggerValue(trigger, true);

    public void UnsetTrigger(CellState trigger) => SetTriggerValue(trigger, false);

    void SetTriggerValue(CellState trigger, bool value = true)
	{
		switch (trigger)
		{
			case CellState.hover:
                hovered = value;
				break;

			case CellState.select:
                selected = value;
				break;

			case CellState.clickable:
                clickable = value;
				break;

			case CellState.pathShown:
                pathShown = value;
				break;

			case CellState.pathHint:
                pathHinted = value;
				break;

			default:
				break;
		}

        UpdateVisualTargets();
	}


    Coroutine updateCoroutine;
    void UpdateVisualTargets()
    {
        //... hover:
        currInteractionTargetSize = unhoveredHexRingSize;
        if (hovered)
            currInteractionTargetSize = hoveredHexRingSize;
		
        if (clickable)
            currInteractionTargetSize = clickableSize;
        
        if (selected)
            currInteractionTargetSize = selectedSize;

        currHoverTargetColor = interactionBaseColor;

        currHoverAlphaTarget = 0f;
        if (hovered)
            currHoverAlphaTarget = hoveredAlpha;
        if (clickable || selected)
            currHoverAlphaTarget = 1f;

        //... path:
        pathTargetSize = pathHiddenSize;
        if (pathShown)
            pathTargetSize = pathShownSize;
        else if (pathHinted)
            pathTargetSize = pathHintSize;

        currPathTargetColor = (pathShown || pathHinted ) ? pathableColor.With(a: 1f) : pathableColor.With(a: 0f);

        currSmoothTime = smoothTime;
    }


	[ReadOnly] public float currSmoothTime = -1f;
	void SmoothChange()
	{
        if (currSmoothTime < 0f)
            return;

        float interactionDampingOffset = UnityEngine.Random.Range(-interactionDampingRange, interactionDampingRange);
        float interactionFrequencyOffset = UnityEngine.Random.Range(-interactionFrequencyRange, interactionFrequencyRange);

        float pathDampingOffset = UnityEngine.Random.Range(-pathDampingRange, pathDampingRange);
        float pathFrequencyOffset = UnityEngine.Random.Range(-pathFrequencyRange, pathFrequencyRange);

        //... hover:
        Springy.Spring(
            ref currInteractionSize,
            ref currInteractionSizeVelocity,
            currInteractionTargetSize,
            interactionDamping + interactionDampingOffset,
            interactionFrequency + interactionFrequencyOffset,
            Time.deltaTime
            );

        float colorLerp = Mathf.Clamp01(currSmoothTime / colorLerpTime);
        
        currHoverAlpha = Mathf.SmoothDamp(
            currHoverAlpha,
            currHoverAlphaTarget,
            ref currHoverAlphaVel,
            currHoverAlphaTarget == 0 ? hoverAlphaSmoothTimeDown : hoverAlphaSmoothTimeUp
            );

        currHoverColor = Color.Lerp(currHoverColor, currHoverTargetColor, colorLerp).With(a: currHoverAlpha);

        //... path:
        Springy.Spring(
            ref pathCurrSize,
            ref pathSizeVelocity,
            pathTargetSize,
            pathDamping + pathDampingOffset,
            pathFrequency + pathFrequencyOffset,
            Time.deltaTime
            );

        pathCurrColor = Color.Lerp(pathCurrColor, currPathTargetColor, Mathf.Clamp01(currSmoothTime / colorLerpTime));

        UpdateMaterialBlock();

        currSmoothTime -= Time.deltaTime;
    }

    void UpdateMaterialBlock()
	{
		//... hover:
		interactionHexSize.floatValue = currInteractionSize.x;
		interactionHexThickness.floatValue = currInteractionSize.y;
		interactionColor.colorValue = currHoverColor;

		hoverBlock.RecordChange(interactionHexSize);
		hoverBlock.RecordChange(interactionHexThickness);
		hoverBlock.RecordChange(interactionColor);

        //... path:
        pathBlockSize.floatValue = pathCurrSize.x;
        pathBlockThickness.floatValue = 1f;
        pathBlockColor.colorValue = pathCurrColor;

        pathBlock.RecordChange(pathBlockColor);
        pathBlock.RecordChange(pathBlockThickness);
        pathBlock.RecordChange(pathBlockSize);
    }

    Sequence seq;
	public void TweenToTarget()
    {
        if (seq.IsActive())
            seq.Kill();

        seq = DOTween.Sequence();

        seq.SetAutoKill();
    }
}
