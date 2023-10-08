using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellStepConfig", menuName = "Grids/CellConfig/CellStep")]
public class CellStepConfig : CellConfig, IExitHandler
{
	public GameObject stepPrefab;

	public void OnExit(CellObject cellObj)
	{
		if (!stepPrefab)
			return;

		var particleStepper = Instantiate(stepPrefab).GetComponent<ParticleStepper>();
		particleStepper.transform.position = cellObj.transform.position;

		//if(cellObj is Character)
		//	Globals.Timelines.Bind(cellObj as Character, particleStepper);
		
		//particleStepper.BindTo(character, true);
	}
}