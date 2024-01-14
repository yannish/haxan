using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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
}
