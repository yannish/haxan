using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Haxan/Event")]
public class GameEventVariable : ScriptableObject
{
    public Action onEventFired;
    //public Action<object> onEventFired;

    public EditorButton fireEventBtn = new EditorButton("FireEvent", true);

    public void FireEvent() => onEventFired?.Invoke();

    public delegate void GameEventDelegate();

    public void RegisterEventListener(GameEventDelegate del)
    //public void RegisterEventListener(Action<object> action)
    {
        onEventFired += () => del();
	}
}
