using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCommand
{

}

public class UnitCommandTimeline : MonoBehaviour
{
    public int length;

    public float itemHeight;
    public float itemWidth = 200f;

    public List<TimeStepSequence> sequences = new List<TimeStepSequence>();
    //public List<FakeCommand> commands = new List<FakeCommand>();
}
