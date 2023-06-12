//using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour 
{
	public static event Action<Health> OnHealthAdded = delegate { };
	public static Action<Health> OnHealthRemoved = delegate { };

	public event Action<int> OnHealthChangedTo = delegate { };
	public event Action OnDeath = delegate { };

	public event Action OnShow = delegate { };
	public event Action OnHide = delegate { };


	//[SerializeField]
	//[ShowInInspector, ReadOnly]
	public int max { get; private set; }

	//[ShowInInspector, ReadOnly]
	public int current { get; private set; }

	public float currentPercent { get { return (float)current / max; } }


	void Decrement() => ChangeBy(-1);

	void Increment() => ChangeBy(1);

	void DoKill() => OnDeath();



	public void ChangeBy(int amount)
	{
		current += amount;
		current = Mathf.Clamp(current, 0, max);

		OnHealthChangedTo(current);

		if (current <= 0)
			OnDeath();
	}

	public void Show() { OnShow(); }
	public void Hide() { OnHide(); }

	private void Start()
	{
		//max = GetComponent<Character>().config.StartingHP;
		current = max;

		OnHealthAdded(this);
	}

	private void OnEnable()
	{
		OnHealthAdded(this);
	}

	private void OnDisable()
	{
		OnHealthRemoved(this);
	}
}
