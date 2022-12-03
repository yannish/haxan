using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


public class CellVisuals : MonoBehaviour
{
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


    [ReadOnly] public bool hovered;
    public EditorButton hoverOnBtn = new EditorButton("HoverOn", true);
    void HoverOn()
    {
        SetTrigger(CellState.hover);
        UpdateVisuals();
    }

    public EditorButton hoverOffBtn = new EditorButton("HoverOff", true);
    void HoverOff()
    {
        UnsetTrigger(CellState.hover);
        UpdateVisuals();
    }
   
    public float hoveredAlpha = 0.5f;
    public HexRingSize hoveredHexRingSize;
    public HexRingSize unhoveredHexRingSize;


    [Header("CLICKABLE:")]
    [ReadOnly] public bool clickable;
    public EditorButton clickableOnBtn = new EditorButton("ClickableOn", true);
    void ClickableOn()
    {
        SetTrigger(CellState.clickable);
        UpdateVisuals();
    }

    public EditorButton clickableOffBtn = new EditorButton("ClickableOff", true);
    void ClickableOff()
    {
        UnsetTrigger(CellState.clickable);
        UpdateVisuals();
    }
    public HexRingSize clickableSize;


    [Header("SELECT:")]
    [ReadOnly] public bool selected;
    public EditorButton selectOnBtn = new EditorButton("SelectOn", true);
    void SelectOn()
    {
        SetTrigger(CellState.select);
        UpdateVisuals();
    }

    public EditorButton selectOffBtn = new EditorButton("SelectOff", true);
    void SelectOff()
    {
        UnsetTrigger(CellState.select);
        UpdateVisuals();
    }
    public HexRingSize selectedSize;


    [Header("PATH:")]
    public MaterialBlockHandle pathBlock;
    public MaterialBlockFloat pathBlockSize;
    public MaterialBlockFloat pathBlockThickness;
    public MaterialBlockColor pathBlockColor;
    public Color pathableColor;

    [ReadOnly] public bool pathHinted;
    public EditorButton pathHintOnBtn = new EditorButton("PathHintOn", true);
    void PathHintOn()
    {
        SetTrigger(CellState.pathHint);
        UpdateVisuals();
    }

    public EditorButton pathHintOffBtn = new EditorButton("PathHintOff", true);
    void PathHintOff()
    {
        UnsetTrigger(CellState.pathHint);
        UpdateVisuals();
    }

    [ReadOnly] public bool pathShown;
    public EditorButton pathShownOnBtn = new EditorButton("PathShownOn", true);
    void PathShownOn()
    {
        SetTrigger(CellState.pathShown);
        UpdateVisuals();
    }

    public EditorButton pathShownOffBtn = new EditorButton("PathShownOff", true);
    void PathShownOff()
    {
        UnsetTrigger(CellState.pathShown);
        UpdateVisuals();
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
    


    [Header("THREAT:")]
    public Color threatColor;



    private void Awake()
	{
        UpdateMaterialBlock();

        UpdateVisuals();
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

        UpdateVisuals();
	}


    Coroutine updateCoroutine;
    void UpdateVisuals()
    {
        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);


        //... hover:
        currInteractionTargetSize = unhoveredHexRingSize;
        if (hovered)
            currInteractionTargetSize = hoveredHexRingSize;
		
        if (clickable)
            currInteractionTargetSize = clickableSize;
        
        if (selected)
            currInteractionTargetSize = selectedSize;

        currHoverTargetColor = interactionBaseColor;

        currHoverAlpha = 0f;
        if (hovered)
            currHoverAlpha = hoveredAlpha;
        if(clickable || selected)
            currHoverAlpha = 1f;

        currHoverTargetColor = currHoverTargetColor.With(a: currHoverAlpha);


        //... path:
        pathTargetSize = pathHiddenSize;
        if (pathShown)
            pathTargetSize = pathShownSize;
        else if (pathHinted)
            pathTargetSize = pathHintSize;

        currPathTargetColor = (pathShown || pathHinted ) ? pathableColor.With(a: 1f) : pathableColor.With(a: 0f);

		updateCoroutine = StartCoroutine(SmoothChange());
	}

    IEnumerator SmoothChange()
	{
        float interactionDampingOffset = UnityEngine.Random.Range(-interactionDampingRange, interactionDampingRange);
        float interactionFrequencyOffset = UnityEngine.Random.Range(-interactionFrequencyRange, interactionFrequencyRange);

        for (float t = 0f; t < smoothTime; t += Time.deltaTime)
        {
            //... hover:
            Springy.Spring(
                ref currInteractionSize, 
                ref currInteractionSizeVelocity, 
                currInteractionTargetSize, 
                interactionDamping + interactionDampingOffset, 
                interactionFrequency + interactionFrequencyOffset, 
                Time.deltaTime
                );

            float colorLerp = Mathf.Clamp01(t / colorLerpTime);
            currHoverColor = Color.Lerp(currHoverColor, currHoverTargetColor, colorLerp);


            //... path:
            Springy.Spring(
                ref pathCurrSize, 
                ref pathSizeVelocity, 
                pathTargetSize,
                interactionDamping + interactionDampingOffset,
                interactionFrequency + interactionFrequencyOffset, 
                Time.deltaTime
                );
            
            pathCurrColor = Color.Lerp(pathCurrColor, currPathTargetColor, Mathf.Clamp01(t / colorLerpTime));
            
            UpdateMaterialBlock();

            yield return 0;
        }
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
        pathBlockThickness.floatValue = pathCurrSize.y;
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

    //public EditorButton setInsetBig = new EditorButton("SetInsetBig", true);
    //   public void SetInsetBig()
    //{
    //       //currHoverTargetInset = unhoveredInset;
    //       StartCoroutine(SmoothChange());
    //       //TweenToTarget();
    //}

    //   public EditorButton setInsetSmall = new EditorButton("SetInsetSmall", true);
    //   public void SetInsetSmall()
    //{
    //       //currHoverTargetInset = hoveredInset;
    //       StartCoroutine(SmoothChange());
    //       //TweenToTarget();
    //   }


    //   public EditorButton setOutsetBig = new EditorButton("SetOutsetBig", true);
    //   public void SetOutsetBig()
    //   {
    //       //currHoverTargetOutset = unhoveredOutset;
    //       StartCoroutine(SmoothChange());
    //       //TweenToTarget();
    //   }

    //   public EditorButton setOutsetSmall = new EditorButton("SetOutsetSmall", true);
    //   public void SetOutsetSmall()
    //   {
    //       //currHoverTargetOutset = hoveredOutset;
    //       StartCoroutine(SmoothChange());
    //       //TweenToTarget();
    //   }
}
