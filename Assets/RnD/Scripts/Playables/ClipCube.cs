using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ClipCube : MonoBehaviour
{
	public enum ScrubCubeState
	{
		UP_AND_DOWN,
		LEFT_AND_RIGHT,
	}

	public ScrubCubeState state = ScrubCubeState.UP_AND_DOWN;

	public AnimationClip upAndDownClip;

	public AnimationClip leftAndRightClip;

	public AnimationClip zRotClip;

	[Header("BITS:")]
	public ClipPlayer clipPlayer;

	private void Start()
	{
		clipPlayer = GetComponent<ClipPlayer>();
		if (clipPlayer == null)
			return;

		clipPlayer.TransitionToRaw(upAndDownClip);
	}

	public EditorButton upDownBtn = new EditorButton("GoToUpDown", true);
	public void GoToUpDown()
	{
		state = ScrubCubeState.UP_AND_DOWN;
		clipPlayer.TransitionToRaw(upAndDownClip);
	}

	public EditorButton leftRightBtn = new EditorButton("GoToLeftRight", true);
	public void GoToLeftRight()
	{
		state = ScrubCubeState.LEFT_AND_RIGHT;
		clipPlayer.TransitionToRaw(leftAndRightClip);
	}

	
	public float duration = 3f;
	public float smoothTimeUp = 0.4f;
	public float smoothTimeDown = 0.4f;
	ClipHandle zRotScrubClip;

	public EditorButton zRotOnBtn = new EditorButton("AddZRot", true);
	public void AddZRot()
	{
		if(zRotScrubClip == null)
			zRotScrubClip = clipPlayer.PlayAdditively(zRotClip, 1f, 1f);

		if (smoothZRotCR != null)
			StopCoroutine(smoothZRotCR);

		smoothZRotCR = StartCoroutine(DoSmoothZRotSpeed(duration, 1f, smoothTimeUp));
	}

	public EditorButton zRotOffBtn = new EditorButton("RemoveZRot", true);
	public void RemoveZRot()
	{
		if (zRotClip == null)
			return;

		if (smoothZRotCR != null)
			StopCoroutine(smoothZRotCR);

		smoothZRotCR = StartCoroutine(DoSmoothZRotSpeed(
			duration, 
			0f, 
			smoothTimeDown, 
			() =>
			{
				clipPlayer.Release(zRotScrubClip);
				zRotScrubClip = null;
			})
			);
	}

	Coroutine smoothZRotCR;
	public float timer;
	public float currZRotSpeed;
	float targetZRotSpeed;
	float zRotVel;
	Action onZRotBlendComplete;
	IEnumerator DoSmoothZRotSpeed(float time, float targetSpeed, float smoothTime, Action onComplete = null)
	{
		timer = time;

		while(timer > 0f)
		{
			//Debug.LogWarning("in loop!");
			currZRotSpeed = Mathf.SmoothDamp(currZRotSpeed, targetSpeed, ref zRotVel, smoothTime);
			if(zRotScrubClip != null && !zRotScrubClip.clipPlayable.IsNull())
				PlayableExtensions.SetSpeed(zRotScrubClip.clipPlayable, currZRotSpeed);
			else
			{
				Debug.LogWarning("something was null for zrot");
			}
			timer -= Time.deltaTime;
			yield return 0;
			//yield return new WaitForEndOfFrame();
		}

		onComplete?.Invoke();
	}

	[Header("CONTROLS:")]
	[Range(0f, 1f)]
	public float zRotSpeed = 1f;
	public void Update()
	{
		//if(zRotScrubClip != null)
		//{
		//	PlayableExtensions.SetSpeed(zRotScrubClip.clipPlayable, zRotSpeed);
		//}
	}
}
