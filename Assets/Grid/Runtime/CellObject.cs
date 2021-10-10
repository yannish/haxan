using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase, ExecuteAlways]
public class CellObject : MonoBehaviour
{
	public HexDirection facing;

	[ReadOnly]
	public FlowController flowController;

	public Transform pivot;

	//private void Awake() => { }

	//... awakenedOverGrid rebinds every cellObject to its cell.
	//... done in Start, because cells are Unbinding themselves on Awake.
    private void Start()
    {
		flowController = GetComponent<FlowController>();

		if (gameObject.activeSelf)
            this.AwakenOverGrid();
    }

	public Cell CurrentCell
	{
		get
		{
			return Globals.Grid.cellObjectBindings.TryGetBinding(this, out Cell foundCell) ?
				foundCell :
				null;
		}
	}


	protected virtual void OnEnable() => this.BindInPlace();   

    protected virtual void OnDisable() => this.Unbind();

	//public virtual void SetFacing(HexDirection newDirection)
	//{
	//	var newFacingDir = dir.ToVector();
	//	cellObj.pivot.rotation = Quaternion.LookRotation(newFacingDir);
	//	cellObj.facing = dir;
	//}
}



public static class CellObjectExtensions
{
	public static void SetFacing(this CellObject cellObj, HexDirection dir)
	{
		var newFacingDir = dir.ToVector();
		if(cellObj.pivot != null)
			cellObj.pivot.rotation = Quaternion.LookRotation(newFacingDir);
		cellObj.facing = dir;
	}

	//public static Cell CurrentCell(this CellObject cellObj)
	//{
	//	if (Globals.Grid.cellObjectBindings.TryGetBinding(cellObj, out Cell foundCell))
	//		return foundCell;
	//	return null;
	//}

	public static Cell NearestCell(this CellObject cellObj)
	{
		Ray ray = new Ray(cellObj.transform.position + Vector3.up * 10f, Vector3.down);
		if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, HexGrid.Mask))
		{
			Cell hitCell = hit.transform.GetComponentInParent<Cell>();
			if ( hitCell != null)
				return hitCell;
		}

		Debug.LogWarning("... nearest cell hit NOTHING: ");

		return null;
	}
}