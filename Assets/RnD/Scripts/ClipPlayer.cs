using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


[Serializable]
public class ScrubClip
{
    public AnimationClip clip;
    public AnimationClipPlayable clipPlayable;
    
    public float scrubTime;
    public float inputWeight;
    public float speed;

    public ScrubClip(PlayableGraph graph, AnimationClip clip)
	{
        this.clip = clip;
        this.speed = 1f;
        this.clipPlayable = AnimationClipPlayable.Create(graph, clip);
	}
}


[RequireComponent(typeof(Animator))]
public class ClipPlayer : MonoBehaviour
{
    public List<AnimationClip> clips = new List<AnimationClip>();

    public List<ScrubClip> scrubClips = new List<ScrubClip>();

    public AnimationClipPlayable[] clipPlayables;

    PlayableGraph graph;

    AnimationMixerPlayable mixerPlayable;

    Animator animator;


    void Start()
	{
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = null;

        //animator = this.gameObject.AddComponent<Animator>();

        graph = PlayableGraph.Create();

        var playableOutput = AnimationPlayableOutput.Create(graph, "Animation", animator);

        mixerPlayable = AnimationMixerPlayable.Create(graph, clips.Count);

        playableOutput.SetSourcePlayable(mixerPlayable);

        clipPlayables = new AnimationClipPlayable[clips.Count];

		for (int i = 0; i < clips.Count; i++)
		{
            var clip = clips[i];

            scrubClips.Add(new ScrubClip(graph, clip));

            //clipPlayables[i] = AnimationClipPlayable.Create(graph, clip);

            //var clipPlayable = AnimationClipPlayable.Create(graph, clip);

            //graph.Connect(clipPlayables[i], 0, mixerPlayable, i);

            //mixerPlayable.SetInputWeight(i, 1f);

            //clipPlayable.Play();
        }

        graph.Play();

        //Animator.en

        //mixerPlayable.SetInputWeight(0, 1f);
	}

    public void SetClip(int clipIndex)
	{
		for (int i = 0; i < clips.Count; i++)
		{
            mixerPlayable.SetInputWeight(i, 0f);
        }

        mixerPlayable.SetInputWeight(clipIndex, 1f);
        mixerPlayable.DisconnectInput(0);

        graph.Connect(scrubClips[clipIndex].clipPlayable, 0, mixerPlayable, 0);

        //clipPlayables[clipIndex].Play();

        //      int inputCount = mixerPlayable.GetInputCount();
        //      for (int i = 0; i < inputCount; i++)
        //{
        //          //graph.Disconnect(i)
        //      }

    }

	private void OnDisable()
	{
        if (!graph.IsValid())
            return;
        graph.Destroy();
	}
}
