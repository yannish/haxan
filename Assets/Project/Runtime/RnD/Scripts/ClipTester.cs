using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClipHandler_OLD))]
public class ClipTester : MonoBehaviour
{
    public List<AnimationClip> clips = new List<AnimationClip>();
    public ClipHandler_OLD clipHandler;
    public float time;

    void Start()
    {
        clipHandler = GetComponent<ClipHandler_OLD>();
    }
}
