using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class GridSnap : MonoBehaviour
{
    public Transform Pivot;
	public HexDirectionFT Facing;

    void OnDrawGizmosSelected()
    {
        if (transform.hasChanged)
            transform.SnapToGrid();
    }
}
