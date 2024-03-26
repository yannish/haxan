using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClipHandler_OTHER_OLD))]
public class ClipTester : MonoBehaviour
{
    public List<AnimationClip> clips = new List<AnimationClip>();
    public ClipHandler_OTHER_OLD clipHandler;
    public float time;

    void Start()
    {
        clipHandler = GetComponent<ClipHandler_OTHER_OLD>();
    }
}
