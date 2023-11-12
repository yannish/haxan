using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder_OLD
{
	public static List<Cell_OLD> exploredCells = new List<Cell_OLD>();
	public static List<Cell_OLD> foundPath = new List<Cell_OLD>();

	public static Dictionary<Cell_OLD, float> shortestDistToSource = new Dictionary<Cell_OLD, float>();
	public static Dictionary<Cell_OLD, Cell_OLD> previousNode = new Dictionary<Cell_OLD, Cell_OLD>();

	public static List<Cell_OLD> GetPath(
		Cell_OLD source,
		Cell_OLD destination,
		List<Cell_OLD> restrictedCells = null
		)
	{
		if (source == null)
			return null;

		Queue<Cell_OLD> nodesToCheck = new Queue<Cell_OLD>();
		nodesToCheck.Enqueue(source);

		exploredCells.Clear();
		shortestDistToSource.Clear();
		previousNode.Clear();
		foundPath.Clear();

		for (int i = 0; i < Globals.Grid.cells.Length; i++)
		{
			shortestDistToSource.Add(Globals.Grid.cells[i], Mathf.Infinity);
			previousNode.Add(Globals.Grid.cells[i], null);
		}

		shortestDistToSource[source] = 0f;

		while(nodesToCheck.Count > 0)
		{
			var inspectedNode = nodesToCheck.Dequeue();
			exploredCells.Add(inspectedNode);

			if(inspectedNode == destination)
			{
				var path = new List<Cell_OLD>();
				path.Add(destination);

				var backNode = previousNode[inspectedNode];
				while (backNode != source && backNode != null)
				{
					path.Add(backNode);
					backNode = previousNode[backNode];
				}

				path.Reverse();
				//foundPath = path;
				return path;
			}

			var neighbours = inspectedNode.validNeighbours;

			foreach(Cell_OLD neighbour in neighbours)
			{
				if (neighbour == null)
					continue;

				if (
					!restrictedCells.IsNullOrEmpty()
					&& restrictedCells.Contains(neighbour)
					)
					continue;

				if (exploredCells.Contains(neighbour) || nodesToCheck.Contains(neighbour))
					continue;

				nodesToCheck.Enqueue(neighbour);

				var tentativeDistance =
					shortestDistToSource[inspectedNode] +
					(neighbour.transform.position - inspectedNode.transform.position).magnitude;

				//... TODO: could have shortestDistToSource asked for a non-existent neighbour key.

				if(
					shortestDistToSource.ContainsKey(neighbour)
					&& tentativeDistance < shortestDistToSource[neighbour]
					)
				{
					shortestDistToSource[neighbour] =
						shortestDistToSource[inspectedNode] +
						(neighbour.transform.position - inspectedNode.transform.position).magnitude;

					previousNode[neighbour] = inspectedNode;
				}
			}
		}

		return null;

	}
}
