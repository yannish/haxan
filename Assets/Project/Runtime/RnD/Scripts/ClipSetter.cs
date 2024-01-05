using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClipHandler))]
public class ClipSetter : MonoBehaviour
{
    [ReadOnly] public ClipHandler clipHandler;
    public AnimationClip clip;

	[Range(0f, 1f)] public float scrubTime;
	public bool setClip = true;

	private void Start()
	{
		clipHandler = GetComponent<ClipHandler>();
	}

	public EditorButton setClipBtn = new EditorButton("SetClip", true);
	public void SetClip()
	{
		clipHandler.SetClip(clip, setClip);
		clipHandler.Scrub(scrubTime);
	}
}
