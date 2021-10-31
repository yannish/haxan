using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
//[ExecuteAlways] //... why?
public class CellObject : MonoBehaviour
{
	[Header("Config")]
	public Sprite icon;
	public Transform pivot;

	[Header("State")]
	public HexDirection facing;

	[Header("Flow")]
	[ReadOnly]
	public FlowController flowController;

	//private void Awake() => { }

	//... awakenedOverGrid rebinds every cellObject to its cell.
	//... done in Start, because cells are Unbinding themselves on Awake.
    protected virtual void Start()
    {
		//Debug.LogWarning("Start on cellobj : " + this.gameObject.name, gameObject);

		flowController = GetComponent<FlowController>();

		if (gameObject.activeSelf)
            this.AwakenOverGrid();
    }

	public Cell currCell
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