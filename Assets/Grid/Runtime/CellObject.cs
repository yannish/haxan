using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[ExecuteAlways] //... why? for OnEnable / OnDisable
public class CellObject : MonoBehaviour
{
	public Transform pivot;

	[Header("CONFIG:")]
	public Sprite icon;

	[ExposedScriptableObject]
	public CellObjectPreset preset;

	[Header("FLOW:")]
	[ReadOnly] public FlowController flowController;

	[Header("STATE:")]
	[ReadOnly] public HexDirection facing;

	[Header("ABILITIES:")]
	public List<Ability> abilities;

	public List<AbilityScrObj> abilityScrObjs = new List<AbilityScrObj>();

	public List<AbilityFlowController> abilityFlows = new List<AbilityFlowController>();

	protected virtual void OnEnable() => this.BindInPlace();

	protected virtual void OnDisable() => this.Unbind();

	private void Awake()
	{
		if (Application.isPlaying)
			abilityScrObjs.ForEach(t => CloneAbility(t));
	}

	//... awakenedOverGrid rebinds every cellObject to its cell.
	//... done in Start, because cells are Unbinding themselves on Awake.
	//... ExecuteAlways means Start is called again when STOPPING play mode... careful.
	protected virtual void Start()
    {
		//Debug.LogWarning("Start on cellobj : " + this.gameObject.name, gameObject);
		flowController = GetComponent<FlowController>();

		if (gameObject.activeSelf)
            this.AwakenOverGrid();

		//List<Ability> instancedAbilities = new List<Ability>();
		//foreach(var ability in abilities)
		//{
		//	var instancedAbility = Instantiate(ability, this.transform);
		//	instancedAbilities.Add(instancedAbility);
		//}
		//abilities = instancedAbilities;

    }

	public Cell_OLD currCell => Globals.Grid.cellObjectBindings.TryGetBinding(this, out Cell_OLD cell) ? cell : null;

	public AbilityFlowController CloneAbility(AbilityScrObj ability)
	{
		var newGameObject = new GameObject(ability.name, typeof(AbilityFlowController));
		newGameObject.transform.SetParent(this.transform);
		newGameObject.hideFlags = HideFlags.DontSaveInEditor;

		var newFlowController = newGameObject.GetComponent<AbilityFlowController>();
		newFlowController.abilityScrObj = ability;
		//abilityFlows.Add(newFlowController);

		return newFlowController;
	}
}



public static class CellObjectExtensions
{
	/// <summary>
	/// Move character.
	/// </summary>
	/// <param name="cellObj"></param>
	/// <param name="newPos"></param>
	/// <param name="inLocalSpace"></param>

	public static void SetVisualPos(this CellObject cellObj, Vector3 newPos, bool inLocalSpace = false)
	{
		if (cellObj.pivot == null)
			return;

		if (inLocalSpace)
			cellObj.pivot.localPosition = newPos;
		else
			cellObj.pivot.position = newPos;
	}

	public static void SetDirectFacing(this CellObject cellObj, Vector3 dir)
	{
		if (cellObj.pivot == null)
			return;

		cellObj.pivot.rotation = Quaternion.LookRotation(dir);
	}

	public static void SetFacing(this CellObject cellObj, HexDirection dir)
	{
		var newFacingDir = dir.ToVector();
		if(cellObj.pivot != null)
			cellObj.pivot.rotation = Quaternion.LookRotation(newFacingDir);
		cellObj.facing = dir;
	}

	public static Cell_OLD NearestCell(this CellObject cellObj)
	{
		Ray ray = new Ray(cellObj.transform.position + Vector3.up * 10f, Vector3.down);
		if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, HexGrid.Mask))
		{
			Cell_OLD hitCell = hit.transform.GetComponentInParent<Cell_OLD>();
			if ( hitCell != null)
				return hitCell;
		}

		Debug.LogWarning("... nearest cell hit NOTHING: ");

		return null;
	}
}