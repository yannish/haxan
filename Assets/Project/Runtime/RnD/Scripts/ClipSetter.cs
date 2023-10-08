using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClipHandler))]
public class ClipSetter : MonoBehaviour
{
    [ReadOnly] public ClipHandler clipHandler;
    public AnimationClip clip;
	[Range(0f, 1f)] public float scrubTime;

	private void Start()
	{
		clipHandler = GetComponent<ClipHandler>();
		clipHandler.SetClip(clip);
		clipHandler.Scrub(scrubTime);
	}
}
