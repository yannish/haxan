using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static bool enemiesCleared
    {
        get
        {
            if (!Application.isPlaying)
                return false;

            if (!CharacterMasterList.sets.ContainsKey(typeof(Enemy)))
                return false;

            return CharacterMasterList.sets[typeof(Wanderer)].Items.Count == 0;
        }
    }

    public static bool wanderersCleared
    {
        get
        {
            if (!Application.isPlaying)
                return false;

            if (!CharacterMasterList.sets.ContainsKey(typeof(Wanderer)))
                return false;

            return CharacterMasterList.sets[typeof(Wanderer)].Items.Count == 0;
        }
    }

    public static bool Ready(this Wanderer wanderer)
	{
        return Globals.ReadyWanderers.Items.Contains(wanderer);
	}
}
