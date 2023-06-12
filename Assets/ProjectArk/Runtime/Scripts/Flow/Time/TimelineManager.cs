using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : MonoBehaviour
	, IPreloadable
{
	public float smoothTime = 0.2f;

	//public List<CharacterTimeline> timelines;
	public Dictionary<Character, CharacterTimeline> timelineLookup = 
		new Dictionary<Character, CharacterTimeline>();

	public void Preload()
	{
		//Globals.A
		Debug.Log("Fireup on timeline manager");

		Events.instance.AddListener<ForwardTimeStep>(ProcessForwardTimeStep);
		Events.instance.AddListener<BackwardTimeStep>(ProcessBackwardTimeStep);

		Globals.ActiveWanderers.onIncrement += AddTimeline;
		Globals.ActiveEnemies.onIncrement += AddTimeline;
	}

	//private void Update()
	//{
		//foreach(var character in timelineLookup.Keys)
		//{
		//	timelineLookup[character].Tick(this);
		//}
	//}

	void AddTimeline(Character character)
	{
		Debug.Log("adding timeline for : " + character.name);

		var newTimelineObj = new GameObject("timeline - " + character.name);
		newTimelineObj.transform.SetParent(transform);
		var newTimeline = newTimelineObj.AddComponent<CharacterTimeline>();
		//newTimeline.ProvideCharacter(character);

		//timelines.Add(newTimeline);
		timelineLookup.Add(character, newTimeline);
	}

	public bool Bind(Character character, TurnStepper stepEffect)
	{
		if (!timelineLookup.ContainsKey(character))
			return false;

		if(timelineLookup.TryGetValue(character, out CharacterTimeline foundTimeline))
		{
			foundTimeline.boundEffects.Add(stepEffect);
			return true;
		}

		return false;
	}  

	void ProcessForwardTimeStep(ForwardTimeStep e)
	{
		if(timelineLookup.TryGetValue(e.character, out CharacterTimeline foundTimeline))
		{
			foundTimeline.StepForward();
		}
	}

	void ProcessBackwardTimeStep(BackwardTimeStep e)
	{
		if (timelineLookup.TryGetValue(e.character, out CharacterTimeline foundTimeline))
		{
			foundTimeline.StepBackward();
		}
	}
}
