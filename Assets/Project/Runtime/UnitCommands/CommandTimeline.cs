using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FakeCommand
{
    public string name;
    public float startTime;
    public float duration;
}

public class CommandTimeline : MonoBehaviour
{
    public List<FakeCommand> fakeInstigatingCommands = new List<FakeCommand>();
    public List<FakeCommand> fakeCommands = new List<FakeCommand>();

    //.... EDITOR WINDOW:
    public float itemHeight = 120f;
    public float itemWidth = 200f;
    public int commandTextSize = 40;
    public Color commandColor; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
