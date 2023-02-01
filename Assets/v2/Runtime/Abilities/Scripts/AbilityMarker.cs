using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMarker : MonoBehaviour
    , IPoolable
{
    public void Play(Vector3 pos, Vector3 normal) { }

    public void Tick() { }

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
    }

    public EditorButton unmarkBtn = new EditorButton("Unmark", true);
    public void Unmark()
    {
        marked = false;
        UpdateVisuals();
    }


    [Header("SIZE:")]
    public float markedSize = 1f;
    public float unmarkedSize = 1f;
    public float sizeFreq;
    public float sizeDamp;
    float currSize;
    float targetSize;
    float sizeVelocity;


    [Header("COLOR:")]
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

	Coroutine updateCoroutine;
    void UpdateVisuals()
	{
        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);

        targetSize = marked ? markedSize : unmarkedSize;
        targetAlpha = marked ? markedColor.a : unmarkedColor.a;

        StartCoroutine(SmoothChange());
	}

    float currSmoothTime;
    IEnumerator SmoothChange()
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
        blockColor.colorValue = markedColor.With(a: currAlpha);
        blockSize.floatValue = currSize;
        matBlockHandle.RecordChange(blockColor);
        matBlockHandle.RecordChange(blockSize);
    }


}
