using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCommand
{

}

public class UnitCommandTimeline : Service<UnitCommandTimeline>
{
    public List<TimeStepSequence> sequences = new List<TimeStepSequence>();
    public List<Unit> units = new List<Unit>();

    Dictionary<Unit, GameObject> unitToRootObject = new Dictionary<Unit, GameObject>();

    [Header("PLAYBACK:")]
    public StepMode mode = StepMode.PAUSED;

    public void StartSequence(
        Unit instigator,
        GameObject seqPrefab, 
        Vector2Int origin, 
        int startingTime
        )
	{
        //GameObject newSequenceHolder = new GameObject(
        //    instigator.name.ToUpper() + " - " + startingTime,
        //    typeof(TimeStepSequence)
        //    );

        //TimeStepSequence timeStepSeq = newSequenceHolder.GetComponent<TimeStepSequence>();

        //newSequenceHolder.transform.SetParent(this.transform);

        GameObject root;
		if (!unitToRootObject.TryGetValue(instigator, out root))
		{
			root = new GameObject(
				instigator.name.ToUpper() + " - " + startingTime,
				typeof(TimeStepSequence)
				);

            unitToRootObject.Add(instigator, root);
		}

        TimeStepSequence newSequence = GameObject.Instantiate(
            seqPrefab,
            Board.OffsetToWorld(origin),
            Quaternion.identity,
            root.transform
            ).GetComponent<TimeStepSequence>();

        newSequence.timeCreated = startingTime;
        newSequence.instigator = instigator;

        sequences.Add(newSequence);
    }

	public void CacheTimelinesForForwardTick() 
	//... ^^ pick from all your timelines which will march ahead for the next step
	{
	
	}

	public void BeginTickForward() 
	//... ^^ prepare those timelines to start ticking forward
	{ 
	
	}

	public void EndTickForward() 
	//... ^^ complete those timelines ticking forward
	{ 
	
	}

	public void TickForward() 
	//... ^^ tick all those timelines forward
	{ 
	
	}

	public void CacheTimelinesForBackwardTick() 
	//... ^^ pick from all your timelines which will march backward for the next step
	{
	
	}

	public void BeginTickBackward() 
	//... ^^ prepare those timelines to start ticking backward
	{ 
	
	}
	
	public void EndTickBackward() 
	//... ^^ complete those timelines ticking backward
	{ 
	
	}

	public void TickBackward() 
	//... ^^ tick all those timelines backward
	{
	
	}


	private void Update()
	{
		switch (mode)
		{
			case StepMode.PAUSED:
				break;
			case StepMode.PLAYING:
				foreach(var sequence in sequences)
				{
					sequence.TickForward();
				}
				break;
			case StepMode.REWINDING:
				foreach (var sequence in sequences)
				{
					sequence.TickBackward();
				}
				break;
			default:
				break;
		}
	}

	public void RespondToCommandBeginTick(Unit unit, UnitCommand command)
	{
        foreach(var sequence in sequences)
		{
			if (sequence.instigator = unit)
			{

			}
		}
	}

	
	//.... EDITOR WINDOW:
	public float itemHeight;
	public float itemWidth = 200f;
}
