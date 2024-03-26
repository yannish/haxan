using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


public interface IScrubConnectable
{

}



[Serializable]
public class ClipHandle
{
	public ClipPlayer player;

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

    public bool IsValid => !clipPlayable.IsNull();
	
	public void Play()
	{

	}

	public void Rewind()
	{

	}

	public void Pause()
	{

	}

    public void ScrubTo(float newTime)
    {
        this.scrubTime = newTime;
        clipPlayable.SetTime(newTime);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
        PlayableExtensions.SetSpeed(clipPlayable, speed);
    }

    public ClipHandle(ClipPlayer player, AnimationClip clip)
	{
		this.player = player;
        this.clip = clip;
        this.speed = 1f;
        this.clipPlayable = AnimationClipPlayable.Create(player.graph, clip);
        this.clipPlayable.Pause();
	}
}

public enum ScrubClipPlaybackMode
{
	PAUSED,
	PLAYING,
	REWINDING
}


public enum ClipPlayerMode
{
	PAUSED,
	PLAYING,
	//REWINDING,
	BLENDING
}


/*
 * TODO:
 * 
 * If a clip is "released", the index in ClipHandle will probably need to be updated.
 * 
 * GOAL:
 * 
 * - Set a clip
 *	- blend into it gradually.
 *	- scrub it along in sync w/ an Op.
 *	- scrub it to a completion time, but, will always have a tail for it to blend out of from there.
 *	
 * 
 * Nicer if this only interfaces w/ Clips? Handles ScrubClip as an internal thing?
 * 
 * Input 0 & 1: Used to transition "main" clips.
 * 
 * Input 0 - X: Used to play added layers. Something "requests" to play a clip addititvely & clipPlayer handles that.
 */


[RequireComponent(typeof(Animator))]
public class ClipPlayer : MonoBehaviour
{
	[Header("STATE:")]
	public ClipPlayerMode mode = ClipPlayerMode.PAUSED;
	public List<ClipHandle> additiveClips = new List<ClipHandle>();
	public List<ClipHandle> scrubClips = new List<ClipHandle>();
	public List<ClipHandle> debugClips = new List<ClipHandle>();

	[Header("CONFIG:")]
	public int mixerClipCount;
	public int scrubClipCount;
	public int additiveMixerClipCount;
	public bool rebindOnGraphChanged;

	//... debug clips:
	[Header("DEBUG CLIPS:")]
    public List<AnimationClip> clips = new List<AnimationClip>();

	public PlayableGraph graph { get; private set; }

	AnimationMixerPlayable mixer;

	AnimationMixerPlayable scrubMixer;

	AnimationMixerPlayable additiveMixer;

	AnimationLayerMixerPlayable layerMixer;

    Animator animator;

	[ReadOnly] public float transitionTimer;
	[ReadOnly] public float transitionTime;
	public ClipHandle currClip;
	public ClipHandle nextClip;

	MethodInfo rebindMethod;
	object[] target;


	void Awake()
	{
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = null;

        graph = PlayableGraph.Create(this.gameObject.name);

        var playableOutput = AnimationPlayableOutput.Create(graph, "Animation", animator);

        mixer = AnimationMixerPlayable.Create(graph, mixerClipCount);

		additiveMixer = AnimationMixerPlayable.Create(graph, additiveMixerClipCount);

		scrubMixer = AnimationMixerPlayable.Create(graph, scrubClipCount);

		layerMixer = AnimationLayerMixerPlayable.Create(graph, 3);

		graph.Connect(mixer, 0, layerMixer, 0);
		graph.Connect(additiveMixer, 0, layerMixer, 1);
		graph.Connect(scrubMixer, 0, layerMixer, 2);

		layerMixer.SetInputWeight(0, 1f);
		layerMixer.SetInputWeight(1, 1f);
		layerMixer.SetInputWeight(2, 1f);

		//Debug.LogWarning(
		//	$"additives: {layerMixer.IsLayerAdditive(0)}," +
		//	$" {layerMixer.IsLayerAdditive(1)}, " +
		//	$"{layerMixer.IsLayerAdditive(2)}"
		//	);

		playableOutput.SetSourcePlayable(layerMixer);

        graph.Play();

		target = new object[] { false };

		rebindMethod = typeof(Animator).GetMethod(
			"Rebind", 
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
			);
	}

	private void Update()
	{
		//... track clips to be stopped
		switch (mode)
		{
			case ClipPlayerMode.PAUSED:
				break;

			case ClipPlayerMode.PLAYING:
				break;

			//case ClipPlayerMode.REWINDING:
			//	break;

			case ClipPlayerMode.BLENDING:
				var normalizedTime = transitionTime > 0f
					? Mathf.Clamp01(transitionTimer / transitionTime)
					: 0f;

				mixer.SetInputWeight(0, normalizedTime);
				mixer.SetInputWeight(1, 1f - normalizedTime);

				if (transitionTimer > 0f)
					transitionTimer -= Time.deltaTime;

				if (normalizedTime == 0f || transitionTimer <= 0f)
				{
					mixer.DisconnectInput(0);
					mixer.DisconnectInput(1);

					mixer.ConnectInput(0, nextClip.clipPlayable, 0);
					mixer.SetInputWeight(0, 1f);
					
					if(currClip != null && !currClip.clipPlayable.IsNull())
					{
						currClip.clipPlayable.Pause();
						graph.DestroyPlayable(currClip.clipPlayable);
					}
					
					currClip = nextClip;
					nextClip = null;

					mode = ClipPlayerMode.PLAYING;
				}
				break;

			default:
				break;
		}

		foreach(var additiveClip in additiveClips)
		{
			mixer.SetInputWeight(additiveClip.index, additiveClip.inputWeight);
		}

		foreach(var scrubClip in scrubClips)
		{
			scrubMixer.SetInputWeight(scrubClip.index, scrubClip.inputWeight);
		}
	}

	public void PlayImmediate(ClipHandle clip)
	{
		mode = ClipPlayerMode.PLAYING;

		currClip = clip;
		clip.clipPlayable.Play();

		mixer.DisconnectInput(0);
		mixer.SetInputWeight(0, 1f);
		graph.Connect(clip.clipPlayable, 0, mixer, 0);
		
	}

	public void TransitionTo(ClipHandle nextClip, float blendTime = 1f, float startTime = 0f)
	{
		mode = ClipPlayerMode.BLENDING;

		transitionTime = nextClip.transitionTime > 0f ? nextClip.transitionTime : blendTime;
		transitionTimer = transitionTime;
		this.nextClip = nextClip;
		if(startTime >= 0f)
			nextClip.clipPlayable.SetTime(startTime);
		nextClip.clipPlayable.Play();
		mixer.SetInputWeight(0, 1f);
		mixer.SetInputWeight(1, 0f);
		graph.Connect(nextClip.clipPlayable, 0, mixer, 1);
	}

	public ClipHandle GetScrubClip(
		AnimationClip clip,
		float startTime = 0f,
		float initialWeight = 1f
		)
	{
		var newScrubClip = new ClipHandle(this, clip);
        newScrubClip.inputWeight = initialWeight;

		//newScrubClip.index = scrubClips.Count;
		
		if(startTime >= 0f)
			newScrubClip.clipPlayable.SetTime(startTime);

		scrubClips.Add(newScrubClip);

		RewireMixer(scrubClips, scrubMixer);

		Debug.LogWarning($"TRYING TO CONNECT TO INDEX:{newScrubClip.index}");

		//graph.Connect(newScrubClip.clipPlayable, 0, scrubMixer, newScrubClip.index);
		scrubMixer.SetInputWeight(newScrubClip.index, initialWeight);

		return newScrubClip;
	}

	private void RewireMixer(List<ClipHandle> clipHandles, AnimationMixerPlayable mixer)
	{
		var mixerInputCount = mixer.GetInputCount();
		for (int i = 0; i < mixerInputCount; i++)
		{
			Debug.LogWarning($"Disconnecting mixer input: {i}");
			mixer.DisconnectInput(i);
			mixer.SetInputWeight(i, 0f);
		}

		mixer.SetInputCount(clipHandles.Count);
		for (int i = 0; i < clipHandles.Count; i++)
		{
			Debug.LogWarning($"... connecting mixer input: {i}");
			mixer.ConnectInput(i, clipHandles[i].clipPlayable, 0);
			mixer.SetInputWeight(i, 1f);
			//Debug.LogWarning($"setting clip index: {i}");
			clipHandles[i].index = i;
		}
	}

	public ClipHandle TransitionToRaw(
		AnimationClip nextClip, 
		float blendTime = 1f, 
		float startTime = 0f, 
		bool playClip = true
		)
	{
		mode = ClipPlayerMode.BLENDING;

		transitionTime = this.nextClip.transitionTime > 0f ? this.nextClip.transitionTime : blendTime;
		transitionTimer = transitionTime;

		var nextScrubClip = new ClipHandle(this, nextClip);
		this.nextClip = nextScrubClip;

		if (startTime >= 0f)
			this.nextClip.clipPlayable.SetTime(startTime);

		if(playClip)
			nextScrubClip.clipPlayable.Play();

		mixer.SetInputWeight(0, 1f);
		mixer.SetInputWeight(1, 0f);

		graph.Connect(this.nextClip.clipPlayable, 0, mixer, 1);

		return nextScrubClip;
	}

	public ClipHandle PlayAdditively(
		AnimationClip clip, 
		float blendTime = 1f,
		float initialWeight = 1f,
		float startTime = 0f
		)
	{
		var newScrubClip = new ClipHandle(this, clip);
		newScrubClip.index = additiveClips.Count;
		newScrubClip.inputWeight = initialWeight;
		newScrubClip.clipPlayable.Play();
		if (startTime >= 0f)
			newScrubClip.clipPlayable.SetTime(startTime);
		additiveClips.Add(newScrubClip);

		graph.Connect(newScrubClip.clipPlayable, 0, additiveMixer, newScrubClip.index);
		additiveMixer.SetInputWeight(newScrubClip.index, initialWeight);

		return newScrubClip;

	}

	public void PlayOneShot()
	{

	}

	public void PlayAdditiveOneShot()
	{

	}

	public void Release(ClipHandle clip)
	{
		if(rebindOnGraphChanged)
			rebindMethod.Invoke(animator, target);

		additiveMixer.SetInputWeight(clip.index, 0f);
		additiveMixer.DisconnectInput(clip.index);
		graph.DestroyPlayable(clip.clipPlayable);
		if (additiveClips.Contains(clip))
			additiveClips.Remove(clip);
	}

	public void ReleaseScrubClip(ClipHandle clipHandle)
	{
		scrubMixer.DisconnectInput(clipHandle.index);
		scrubClips.Remove(clipHandle);
		graph.DestroyPlayable(clipHandle.clipPlayable);

		RewireMixer(scrubClips, scrubMixer);
	}

	public void PlayPose()
	{
		//var animationPlayable = new AnimationClipPlayable();
	}
	

	private void OnDisable()
	{
        if (!graph.IsValid())
            return;
        graph.Destroy();
	}

}
