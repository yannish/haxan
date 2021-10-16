using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTester : MonoBehaviour
{
    public Cell startCell;
    public Cell destCell;

    public List<Cell> foundPath = new List<Cell>();

    [ContextMenu("Try Path")]
    public void TryPath()
	{
        foundPath = Pathfinder.GetPath(startCell, destCell);
	}
}
