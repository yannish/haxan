using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTimeline : MonoBehaviour
{
	//[ReadOnly, SerializeField] private Character boundCharacter;

	public List<TurnStepper> boundEffects = new List<TurnStepper>();

	//[ReadOnly]
	public int birthStep;
	[ReadOnly] public int currStep;
	//[ReadOnly] public float currTime;
	//[ReadOnly] public float targetTime;
	//[ReadOnly] public float velocity;


	//public void ProvideCharacter(Character character)
	//{
	//	boundCharacter = character;
	//}

	//public void Tick(TimelineManager timelineManager)
	//{
		//currTime = Mathf.SmoothDamp(
		//	currTime,
		//	targetTime,
		//	ref velocity,
		//	timelineManager.smoothTime
		//	);

		//for (int i = 0; i < boundEffects.Count; i++)
		//{
			
		//}

		//for (int i = 0; i < boundEffects.Count; i++)
		//{
		//	boundEffects[i].Tick(currTime);
		//}
	//}

	public void StepForward()
	{
		currStep++;
		//targetTime = currStep;

		foreach(var effect in boundEffects)
		{
			effect.StepForward();
		}
	}

	public void StepBackward()
	{
		currStep--;

		List<TurnStepper> effectsToUnbind = new List<TurnStepper>();

		foreach (var effect in boundEffects)
		{
			effect.StepBackward();
			if (effect.markedForDeath)
				effectsToUnbind.Add(effect);
		}

		foreach(var effectToUnbind in effectsToUnbind)
		{
			boundEffects.Remove(effectToUnbind);
		}
	}
}
