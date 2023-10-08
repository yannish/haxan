using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[Serializable]
public class ClipHandle
{
    public AnimationClip clip;
    public AnimationClipPlayable clipPlayable;

    public double scrubTime;
    public float inputWeight;
    public float speed;

    public ClipHandle(AnimationClip clip, double scrubTime = 0, float weight = 1f, float speed = 1f)
	{
        this.clip = clip;
        this.scrubTime = scrubTime;
        this.inputWeight = weight;
        this.speed = speed;
	}
}

[RequireComponent(typeof(Animator))]
public class ClipHandler : MonoBehaviour
{
    PlayableGraph graph;

    public AnimationMixerPlayable mixerPlayable;

    public AnimationPlayableOutput playableOutput;

    Animator animator;

    public const int NUM_MIXER_INPUTS = 2;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = null;

        graph = PlayableGraph.Create();

        playableOutput = AnimationPlayableOutput.Create(graph, "Animation", animator);

        mixerPlayable = AnimationMixerPlayable.Create(graph, NUM_MIXER_INPUTS);

        playableOutput.SetSourcePlayable(mixerPlayable);

        graph.Play();
    }

    public ClipHandle currClipHandle;

    public void SetClip(AnimationClip clip, bool playClip = false)
	{
        Debug.LogWarning("SET CLIP: " + clip.name);

        var newClipHandle = new ClipHandle(clip);

        newClipHandle.clipPlayable = AnimationClipPlayable.Create(graph, clip);
        mixerPlayable.DisconnectInput(0);
        graph.Connect(newClipHandle.clipPlayable, 0, mixerPlayable, 0);

        if (playClip)
            newClipHandle.clipPlayable.Play();
        else
            newClipHandle.clipPlayable.Pause();

        currClipHandle = newClipHandle;
	}

    public void Scrub(float t)
	{
        if (currClipHandle != null)
            currClipHandle.clipPlayable.SetTime((double)t);
	}

    public void SetMainWeight(float weight) => playableOutput.SetWeight(weight);

    public float GetMainWeight() => playableOutput.IsOutputValid() ? playableOutput.GetWeight() : -1f;

	private void OnDestroy()
	{
        if (graph.IsValid())
            graph.Destroy();
	}
}
