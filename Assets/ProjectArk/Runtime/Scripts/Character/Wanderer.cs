using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(WandererFlowController))]
public class Wanderer : Character
{
	protected override void OnEnable()
	{
		Globals.ActiveWanderers?.Add(this);
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		Globals.ActiveWanderers?.Remove(this);
		base.OnDisable();
	}

	protected override void Start()
	{
		base.Start();
	}

	private void Awake()
	{
		if (!Application.isPlaying)
			return;

		abilityScrObjs.ForEach(t => abilityFlows.Add(CloneAbility(t)));

		if(movementAbilityScrObj != null)
			movementAbilityFlow = CloneAbility(movementAbilityScrObj);

		//if (movementAbilityScrObj)
		//{
		//	var newGameObject = new GameObject(movementAbilityScrObj.name, typeof(AbilityFlowController));
		//	newGameObject.transform.SetParent(this.transform);
		//	newGameObject.hideFlags = HideFlags.DontSaveInEditor;

		//	var newFlowController = newGameObject.GetComponent<AbilityFlowController>();
		//	newFlowController.abilityScrObj = movementAbilityScrObj;

		//	movementAbilityFlow = newFlowController;
		//}
	}


	[ReadOnly, SerializeField] WandererFlowController _flow;
	public WandererFlowController flow
	{
		get
		{
			if (_flow == null)
				_flow = GetComponent<WandererFlowController>();
			return _flow;
		}
	}
}
