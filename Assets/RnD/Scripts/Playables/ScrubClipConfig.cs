using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScrubClipConfig", menuName ="Playables/ScrubClipConfig")]
public class ScrubClipConfig : ScriptableObject
{
	public AnimationClip clip;
	public float transitionTime;
}
