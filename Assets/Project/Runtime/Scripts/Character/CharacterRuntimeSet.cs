using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Runtime Set/Character Runtime Set")]
public class CharacterRuntimeSet : RuntimeSet<Character>
{
    public override void Add(Character thing)
    {
        if(CharacterMasterList.sets.ContainsKey(thing.GetType()))
            CharacterMasterList.sets.Add(thing.GetType(), this);

        base.Add(thing);
    }
}

public static class CharacterMasterList
{
    public static Dictionary<Type, CharacterRuntimeSet> sets = new Dictionary<Type, CharacterRuntimeSet>();

    public static Wanderer Player
    {
        get
        {
            if (sets.ContainsKey(typeof(Wanderer)))
                if (sets[typeof(Wanderer)].Items != null)
                    return sets[typeof(Wanderer)].Items[0] as Wanderer;

            return null;
        }
    }
}