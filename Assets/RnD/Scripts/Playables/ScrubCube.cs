using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

#pragma warning disable 0168  // variable declared but not used.
#pragma warning disable 0219  // variable assigned but not used.
#pragma warning disable 0414  // private field assigned but not used.

public class ScrubCube : MonoBehaviour
{
	public ClipPlayer clipPlayer;

	public AnimationClip leftRightClip;
	public float leftRightSpeed;
	public float leftRightBlendIn;

	public AnimationClip upDownClip;
	public float upDownSpeed;
	public float upDownBlendIn;

	ClipHandle upDownScrubClip;
	ClipHandle leftRightScrubClip;



	private void Awake()
	{
		clipPlayer = GetComponent<ClipPlayer>();
	}

	public EditorButton leftRightBtn = new EditorButton("StartLeftRight", true);
	Coroutine leftRightCR;
	public void StartLeftRight()
	{
		if (leftRightCR != null)
			StopCoroutine(leftRightCR);

		leftRightCR = StartCoroutine(SmoothClipScrub(leftRightClip, leftRightSpeed));
	}

	public EditorButton upDownBtn = new EditorButton("StartUpDown", true);
	Coroutine upDownCR;
	public void StartUpDown()
	{
		if (upDownCR != null)
			StopCoroutine(upDownCR);

		upDownCR = StartCoroutine(SmoothClipScrub(upDownClip, upDownSpeed));
	}

	//public float duration;
	//public float currTimer;
	IEnumerator SmoothClipScrub(AnimationClip clip, float speed)
	{
		var clipHandle = clipPlayer.GetScrubClip(clip, initialWeight: 1f);

		var duration = clipHandle.clip.length / speed;
		var currTimer = duration;

		while(currTimer > 0f)
		{
			var normalizedTime = 1f - Mathf.Clamp01(currTimer / duration);
			clipHandle.clipPlayable.SetTime(normalizedTime);
			currTimer -= Time.deltaTime;
			yield return 0;
		}

		clipPlayer.ReleaseScrubClip(clipHandle);
		upDownScrubClip = null;
	}


	[Header("LOOP:")]
	public AnimationClip loopClip;
	ClipHandle loopClipHandle;
	[Range(0f, 1f)]
	public float loopWeight;

	public EditorButton playLoopBtn = new EditorButton("PlayLoop", true);
	public void PlayLoop() => playLoopCR = StartCoroutine(DoPlayLoop());

	Coroutine playLoopCR;
	public IEnumerator DoPlayLoop()
	{
		float t = 0f;
		loopClipHandle = clipPlayer.GetScrubClip(loopClip);
		while (true)
		{
			t += Time.deltaTime;
			t = Mathf.Repeat(t, 1f);
			loopClipHandle.clipPlayable.SetTime(t);
			loopClipHandle.inputWeight = loopWeight;
			yield return 0;
		}
	}

	public EditorButton stopLoopBtn = new EditorButton("StopLoop", true);
	public void StopLoop()
	{
		clipPlayer.ReleaseScrubClip(loopClipHandle);
		StopCoroutine(playLoopCR);
	}
}
