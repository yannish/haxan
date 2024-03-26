using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClipHandler_OTHER_OLD))]
public class ClipSetter : MonoBehaviour
{
    [ReadOnly] public ClipHandler_OTHER_OLD clipHandler;
    public AnimationClip clip;
	[Range(0f, 1f)] public float scrubTime;

	private void Start()
	{
		clipHandler = GetComponent<ClipHandler_OTHER_OLD>();
		clipHandler.SetClip(clip);
		clipHandler.Scrub(scrubTime);
	}
}
