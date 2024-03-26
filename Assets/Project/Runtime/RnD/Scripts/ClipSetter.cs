using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClipHandler_OLD))]
public class ClipSetter : MonoBehaviour
{
    [ReadOnly] public ClipHandler_OLD clipHandler;
    public AnimationClip clip;

	[Range(0f, 1f)] public float scrubTime;
	public bool setClip = true;

	private void Start()
	{
		clipHandler = GetComponent<ClipHandler_OLD>();
	}

	public EditorButton setClipBtn = new EditorButton("SetClip", true);
	public void SetClip()
	{
		clipHandler.SetClip(clip, setClip);
		clipHandler.Scrub(scrubTime);
	}
}
