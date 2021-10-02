using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
	public static List<Cell> exploredCells = new List<Cell>();
	public static List<Cell> foundPath = new List<Cell>();

	public static Dictionary<Cell, float> shortestDistToSource = new Dictionary<Cell, float>();
	public static Dictionary<Cell, Cell> previousNode = new Dictionary<Cell, Cell>();

	public static List<Cell> GetPath(
		Cell source,
		Cell destination,
		List<Cell> restrictedCells = null
		)
	{
		if (source == null)
			return null;

		Queue<Cell> nodesToCheck = new Queue<Cell>();
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
				var path = new List<Cell>();
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

			foreach(Cell neighbour in neighbours)
			{
				if(neighbour != null)
				{
					if(
						!restrictedCells.IsNullOrEmpty()
						&& restrictedCells.Contains(neighbour)
						)
							continue;

					if(!exploredCells.Contains(neighbour) && !nodesToCheck.Contains(neighbour))
					{
						nodesToCheck.Enqueue(neighbour);

						var tentativeDistance =
							shortestDistToSource[inspectedNode] +
							(neighbour.transform.position - inspectedNode.transform.position).magnitude;

						if(tentativeDistance < shortestDistToSource[neighbour])
						{
							shortestDistToSource[neighbour] =
								shortestDistToSource[inspectedNode] +
								(neighbour.transform.position - inspectedNode.transform.position).magnitude;

							previousNode[neighbour] = inspectedNode;
						}
					}
				}
			}
		}

		return null;

	}
}
