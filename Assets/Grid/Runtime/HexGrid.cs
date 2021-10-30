using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexGrid : MonoBehaviour
{
	public const string GridLayer = "Grid";

	public static LayerMask Mask { get { return LayerMask.GetMask(GridLayer); } }

	public CellObjectBindingMap cellObjectBindings = new CellObjectBindingMap();

    public Dictionary<HexCoordinates, Cell> coordCellLookup = new Dictionary<HexCoordinates, Cell>();

	public Cell[] cells;
}

public static class GridActions
{
    public static void RefreshCellBindings()
    {
        if (Globals.Grid == null)
            return;

        Debug.Log("REFRESHING CELL BINDINGS");

        Globals.Grid.cellObjectBindings.Clear();

        var allCellObjs = GameObject.FindObjectsOfType<CellObject>();
        foreach(var cellObj in allCellObjs)
        {
            cellObj.AwakenOverGrid();
        }
    }

	public static void AwakenOverGrid(this CellObject cellObject)
	{
		Debug.LogWarning("Awakening " + cellObject.name + " over grid...", cellObject);

		Cell foundCell = cellObject.pivot.position.PollGrid();
		if(foundCell != null)
		{
			cellObject.MoveAndBindTo(foundCell);
		}
	}

	public static bool MoveAndBindTo(this CellObject cellObj, Cell cell)
	{
		if(!cell.IsBound())
		{
			cellObj.Unbind();
			cellObj.transform.position = cell.occupantPivot.position;
			cellObj.Bind(cell);
			return true;
		}

		//Debug.LogWarning("Tried to move & bind to an occupied cell.", cell.gameObject);

		return false;
	}

	public static void MoveAndFace(this CellObject cellObj, Cell cell, HexDirection dir)
	{
		cellObj.transform.position = cell.occupantPivot.position;
		//cellObj.transform.rotation.SetLookRotation()
		//Quaternion.LookRotation( dir.ToAngle
		cellObj.transform.rotation = Quaternion.LookRotation(dir.ToVector());
	}

	public static Cell PollGrid(this Vector3 position)
	{
		Ray checkRay = new Ray(position + Vector3.up * 1.8f, Vector3.down);
		RaycastHit hit;

		if (Physics.Raycast(checkRay, out hit, Mathf.Infinity, HexGrid.Mask))
		{
			var cell = hit.collider.GetComponentInParent<Cell>();
			if (cell)
				return cell;
		}

		return null;
	}
}