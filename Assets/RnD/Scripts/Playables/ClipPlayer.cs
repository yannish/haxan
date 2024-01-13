using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


public interface IScrubConnectable
{

}

[Serializable]
public class ScrubClip
{
	//... config:
    public AnimationClip clip;

	public float transitionTime;


	//... state:
	public AnimationClipPlayable clipPlayable;

	public ScrubClipPlaybackMode mode;

	public double scrubTime;
    
	public float inputWeight;
    
	public float speed;
	
    public int index;

	
	
	public void Play()
	{

	}

	public void Rewind()
	{

	}

	public void Pause()
	{

	}

    public ScrubClip(PlayableGraph graph, AnimationClip clip)
	{
        this.clip = clip;
        this.speed = 1f;
        this.clipPlayable = AnimationClipPlayable.Create(graph, clip);
        this.clipPlayable.Pause();
	}
}

public enum ScrubClipPlaybackMode
{
	PAUSED,
	PLAYING,
	REWINDING
}


[RequireComponent(typeof(Animator))]
public class ClipPlayer : MonoBehaviour
{
    public List<AnimationClip> clips = new List<AnimationClip>();

    public List<ScrubClip> scrubClips = new List<ScrubClip>();

    public AnimationClipPlayable[] clipPlayables;

	public PlayableGraph graph { get; private set; }

	AnimationMixerPlayable mixerPlayable;

    Animator animator;




	void Start()
	{
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = null;

        graph = PlayableGraph.Create(this.gameObject.name);

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

			//graph.Connect(scrubClips[i].clipPlayable, 0, mixerPlayable, i);

			//mixerPlayable.SetInputWeight(i, 1f);

			//scrubClips[i].clipPlayable.Play();
		}

        graph.Play();

		if(!scrubClips.IsNullOrEmpty() && scrubClips[0] != null)
			PlayClip(scrubClips[0]);
	}

	//public ScrubClip RegisterClip(AnimationClip clip)
	//{
	//	var newScrubClip = new ScrubClip(graph, clip);
	//	return newScrubClip;
	//}

	//public void SetClip(ScrubClip scrubClip)
	//{

	//}

	private void LateUpdate()
	{
		//... track clips to be stopped
		if(transitionTimer > 0f)
		{
			transitionTimer -= Time.deltaTime;
			var normalizedTime = Mathf.Clamp01(transitionTimer / transitionTime);
			mixerPlayable.SetInputWeight(0, normalizedTime);
			mixerPlayable.SetInputWeight(1, 1f - normalizedTime);

			if(transitionTimer <= 0f)
			{
				mixerPlayable.SetInputWeight(0, 1f);
				mixerPlayable.ConnectInput(0, nextClip.clipPlayable, 0);
				mixerPlayable.DisconnectInput(1);
				currClip = nextClip;
				nextClip = null;
			}
		}
	}

	public void SetClip(int clipIndex)
	{
        //currClip = clipIndex;

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

	public void PlayClip(ScrubClip clip)
	{
		currClip = clip;
		clip.clipPlayable.Play();

		mixerPlayable.DisconnectInput(0);
		mixerPlayable.SetInputWeight(0, 1f);
		graph.Connect(clip.clipPlayable, 0, mixerPlayable, 0);

		//Debug.LogWarning($"mixer input count: { mixerPlayable.GetInputCount()}");

		//if(mixerPlayable.GetInputCount() > 1)
		//	mixerPlayable.DisconnectInput(1);
		
	}

	[ReadOnly] public float transitionTimer;
	[ReadOnly] public float transitionTime;
	public ScrubClip currClip;
	public ScrubClip nextClip;
	//[ReadOnly] public float transitionTime;
	public void TransitionTo(ScrubClip nextClip)
	{
		transitionTimer = nextClip.transitionTime;
		transitionTime = nextClip.transitionTime;
		this.nextClip = nextClip;
		mixerPlayable.SetInputWeight(0, 1f);
		mixerPlayable.SetInputWeight(1, 0f);
		graph.Connect(nextClip.clipPlayable, 0, mixerPlayable, 1);
	}

 //   public void Scrub(int clipIndex, double normalizedClipTime)
	//{
 //       //Debug.LogWarning("SCRUB: " + clipTime);

 //       if (currClip != clipIndex)
 //           SetClip(clipIndex);

 //       var scrubClip = scrubClips[clipIndex];
 //       scrubClip.scrubTime = normalizedClipTime * scrubClip.clip.length;
 //       scrubClip.clipPlayable.SetTime(scrubClip.scrubTime);
	//}

	private void OnDisable()
	{
        if (!graph.IsValid())
            return;
        graph.Destroy();
	}
}
