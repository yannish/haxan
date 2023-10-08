using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Ghol
{
    public int hp;
}

[Serializable]
public class Warden : Ghol
{
    public float roarCooldown;
}

[Serializable]
public class Thrall : Ghol
{
    public bool hasAxe;
}

[Serializable]
public class Wight : Ghol
{
    public int pusSacks;
}

[Serializable]
public class MythGameState
{
    [SerializeReference]
    public List<Ghol> ghols = new List<Ghol>();
}

public class PolymorphicListRunner : MonoBehaviour
{
    //[SerializeReference]
    //public List<Ghol> ghols = new List<Ghol>();

    [Multiline(10)]
    public string jsonBlorb;

    public MythGameState gameState;

	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.S))
            Save();

        if (Input.GetKeyDown(KeyCode.L))
            Load();
    }

	private void Load()
	{
        gameState = JsonUtility.FromJson<MythGameState>(jsonBlorb);
	}

	void Save()
    {
        gameState.ghols.Add(new Warden() { roarCooldown = 7f, hp = 5});
        gameState.ghols.Add(new Wight() { pusSacks = 3, hp = 10 });
        gameState.ghols.Add(new Thrall() { hasAxe = true, hp = 2 });

        jsonBlorb = JsonUtility.ToJson(gameState);
    }
}
