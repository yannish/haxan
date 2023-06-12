using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStepTester : MonoBehaviour
{
	public GameObject stepperPrefab;
	public Character boundCharacter;

	public float stepSize = 2f;
	private Vector3 currPos;



	private void Start()
	{
		var turnStepper = stepperPrefab.GetComponent<TurnStepper>();
		//Globals.Timelines.Bind(boundCharacter, turnStepper);
		//turnStepper?.BindTo(boundCharacter);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.N))
			StepForward();

		if (Input.GetKeyDown(KeyCode.B))
			StepBackward();
	}

	private void StepBackward()
	{
		Debug.Log("Stepping backward");
		Events.instance.Raise(new BackwardTimeStep(boundCharacter));
	}

	private void StepForward()
	{
		Debug.Log("Stepping forward");
		Events.instance.Raise(new ForwardTimeStep(boundCharacter));
	}
}
