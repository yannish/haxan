using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMarker : MonoBehaviour
    , IPoolable
{
    public void Play(int custom) => Mark();

    public void Stop(int stopParams) => Unmark();

    public void Tick() => SmoothChange();

    public bool IsProcessing() => marked || currSmoothTime > 0f;

    public Action onReturnToPool;
    public void Return()
    {
        onReturnToPool?.Invoke();
        onReturnToPool = null;
    }



    public FloatSpring sizeSpring;

    public MaterialBlockHandle matBlockHandle;
    public MaterialBlockColor blockColor;
    public MaterialBlockFloat blockSize;

    public bool marked;
    public EditorButton markBtn = new EditorButton("Mark", true);
    public void Mark()
    {
        marked = true;
        UpdateVisuals();
        //StartCoroutine(DoSmoothChange());
    }

    public EditorButton unmarkBtn = new EditorButton("Unmark", true);
    public void Unmark()
    {
        marked = false;
        UpdateVisuals();
        //StartCoroutine(DoSmoothChange());
    }


    [Header("SIZE:")]
    public bool doSize = true;
    public float markedSize = 1f;
    public float unmarkedSize = 1f;
    public float sizeFreq;
    public float sizeDamp;
    float currSize;
    float targetSize;
    float sizeVelocity;


    [Header("COLOR:")]
    public bool doColor = true;
    public Color markedColor;
    public Color unmarkedColor;
    public float colorSmoothTime;
    float currAlpha;
    float targetAlpha;
    float alphaVelocity;



    [Header("SMOOTHING:")]
    public float smoothTime = 2f;

	private void Awake()
	{
        currSize = unmarkedSize;
        currAlpha = 0;

        UpdateMaterialBlock();
	}

	//Coroutine updateCoroutine;
    void UpdateVisuals()
	{
        //if (updateCoroutine != null)
        //    StopCoroutine(updateCoroutine);

        targetSize = marked ? markedSize : unmarkedSize;
        targetAlpha = marked ? markedColor.a : unmarkedColor.a;

        currSmoothTime = smoothTime;
	}

    float currSmoothTime;
    void SmoothChange()
	{
        if(currSmoothTime > 0f)
		{
            if(doColor)
                currAlpha = Mathf.SmoothDamp(currAlpha, targetAlpha, ref alphaVelocity, colorSmoothTime);
            
            if(doSize)
                Springy.Spring(ref currSize, ref sizeVelocity, targetSize, sizeDamp, sizeFreq, Time.deltaTime);

            currSmoothTime -= Time.deltaTime;

            UpdateMaterialBlock();
        }
    }

	IEnumerator DoSmoothChange()
	{
		for (currSmoothTime = smoothTime; currSmoothTime > 0f; currSmoothTime -= Time.deltaTime)
		{
			currAlpha = Mathf.SmoothDamp(currAlpha, targetAlpha, ref alphaVelocity, colorSmoothTime);
			Springy.Spring(ref currSize, ref sizeVelocity, targetSize, sizeDamp, sizeFreq, Time.deltaTime);

			UpdateMaterialBlock();

			yield return 0;
		}
	}

	void UpdateMaterialBlock()
	{
		if (doColor)
		{
            blockColor.colorValue = markedColor.With(a: currAlpha);
            matBlockHandle.RecordChange(blockColor);
        }

		if (doSize)
		{
            blockSize.floatValue = currSize;
            matBlockHandle.RecordChange(blockSize);
		}
    }


}
