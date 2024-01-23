using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledIndicator : PooledMonoBehaviour
{
	public override void Play(int custom = 0)
	{
		//Debug.LogWarning("PLAYED ON POOLED INDICATOR: " + this.gameObject.name);
		base.Play(custom);
		Display();
	}

	public override void Tick()
	{
		SmoothChange();
		base.Tick();
	}

	public override void Stop(int stopParams = 0)
	{
		base.Stop(stopParams);
		Hide();
	}

	protected override void OnEnable()
	{
		//Debug.LogWarning("ENABLED POOLED INDICATOR: " + this.gameObject.name);
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		//Debug.LogWarning("DISABLED POOLED INDICATOR: " + this.gameObject.name);
		base.OnDisable();
	}


	[Header("STATE:")]
	[ReadOnly] public bool isDisplaying;
	[ReadOnly] public bool isValid;
	[ReadOnly] public float currSmoothTime;
	public override bool IsProcessing()
	{
		bool result = isDisplaying || currSmoothTime > 0f;
		//Debug.LogWarning("... and now currSmoothTime: " + currSmoothTime + ", RESULT: " + result + " on " + this.gameObject.name);
		return result;
	}

	public EditorButton DisplayBtn = new EditorButton("Display", true);
	public void Display()
	{
		//Debug.LogWarning("Displaying pooled indicator.");
		isDisplaying = true;
		UpdateVisualTargets();
	}

	public EditorButton HideBtn = new EditorButton("Hide", true);
	public void Hide()
	{
		isDisplaying = false;
		UpdateVisualTargets();
	}

	public EditorButton ValidBtn = new EditorButton("SetValid", true);
	public void SetValid()
	{
		isValid = true;
		UpdateVisualTargets();
	}
	
	public EditorButton InvalidBtn = new EditorButton("SetInvalid", true);
	public void SetInvalid()
	{
		isValid = false;
		UpdateVisualTargets();
	}


	[Header("COLOR:")]
	public MaterialBlockHandle blockHandle;
	public Color currColor;
	public MaterialBlockColor blockColor;

	[ReadOnly] public float currAlpha;
	[ReadOnly] public float currAlphaTarget;
	[ReadOnly] public float currAlphaVel;
	
    public float totalSmoothTime;
	public float alphaSmoothTime;
	public float colorLerpTime = 1f;

	void SmoothChange()
	{
		if (currSmoothTime < 0f)
			return;

		float colorLerp = Mathf.Clamp01(currSmoothTime / colorLerpTime);

		currAlpha = Mathf.SmoothDamp(currAlpha, currAlphaTarget, ref currAlphaVel, alphaSmoothTime);

		blockColor.colorValue = currColor.With(a: currAlpha);
		blockHandle.RecordChange(blockColor);

		currSmoothTime -= Time.deltaTime;

		//blockColor.colorValue = Color.Lerp(blockColor.colorValue)
	}

	void UpdateVisualTargets()
	{
		currAlphaTarget = 0f;
		if (isDisplaying)
			currAlphaTarget = 1f;

		currSmoothTime = totalSmoothTime;

		//Debug.LogWarning("currSmoothTime: " + currSmoothTime);
	}
}
