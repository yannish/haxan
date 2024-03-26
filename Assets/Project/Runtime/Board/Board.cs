using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Linq;
using BOG;

public static class BoardMetrics
{
	public static Vector2Int ToOffset(this Vector3 worldPos)
	{
		float3 cartesian = new float3(worldPos.x, worldPos.z, 1f);
		float2 axialFrac = math.mul(Board.CartesianToAxial, cartesian).xy;
		Vector2Int axial = new Vector2Int(Mathf.RoundToInt(axialFrac.x), Mathf.RoundToInt(axialFrac.y));
		return AxialToOffset(axial);
	}

	public static Vector2Int ToAxial(this Vector2Int offset)
	{
		return new Vector2Int(
			offset.x,
			offset.y - (offset.x - (offset.x & 1)) / 2
		);
	}

	public static Vector3 ToWorld(this Vector2Int offsetPos)
	{
		Vector2Int axial = offsetPos.ToAxial();
		float2 cartesian = math.mul(
			Board.AxialToCartesian,
			new float3(axial.x, axial.y, 1f)
		).xy;
		return new Vector3(cartesian.x, 0f, cartesian.y);
	}

	public static readonly float3x3 CartesianToAxial = math.mul(
		new float3x3(
			2f / (3f * Cell.OuterRadius), 0f, 0f,
			-1f / (3f * Cell.OuterRadius), Mathf.Sqrt(3f) / (3f * Cell.OuterRadius), 0f,
			0f, 0f, 1f
		),
			new float3x3(
				1f, 0f, -Cell.OuterRadius,
				0f, 1f, -Cell.InnerRadius,
				0f, 0f, 1f
			)
		);

	public static readonly float3x3 AxialToCartesian = math.mul(
		new float3x3(
			1f, 0f, Cell.OuterRadius,
			0f, 1f, Cell.InnerRadius,
			0f, 0f, 1f
		),
		new float3x3(
			1.5f * Cell.OuterRadius, 0f, 0f,
			Cell.InnerRadius, Cell.InnerRadius * 2f, 0f,
			0f, 0f, 1f
		)
	);

	public static Vector2Int AxialToOffset(Vector2Int axial)
	{
		return new Vector2Int(
			axial.x,
			axial.y + (axial.x - (axial.x & 1)) / 2
		);
	}

	public static string ToCoordString(this Vector2Int coord)
	{
		return new string($"({coord.x},{coord.y})");
	}
}

//[InitializeOnLoad]
public class Board
{
	public bool logMouseDebug;

	public static int currTimeStep = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnSceneLoad()
	{
        Debug.LogWarning("SCENE LOADED");
	}

    public static readonly float3x3 CartesianToAxial = math.mul(
        new float3x3(
            2f / (3f * Cell.OuterRadius), 0f, 0f,
            -1f / (3f * Cell.OuterRadius), Mathf.Sqrt(3f) / (3f * Cell.OuterRadius), 0f,
            0f, 0f, 1f
        ),
        new float3x3(
            1f, 0f, -Cell.OuterRadius,
            0f, 1f, -Cell.InnerRadius,
            0f, 0f, 1f
        )
    );
    public static readonly float3x3 AxialToCartesian = math.mul(
        new float3x3(
            1f, 0f, Cell.OuterRadius,
            0f, 1f, Cell.InnerRadius,
            0f, 0f, 1f
        ),
        new float3x3(
            1.5f * Cell.OuterRadius, 0f, 0f,
            Cell.InnerRadius, Cell.InnerRadius * 2f, 0f,
            0f, 0f, 1f
        )
    );


    public static Vector2Int WorldToOffset(Vector3 worldPos)
    {
        float3 cartesian = new float3(worldPos.x, worldPos.z, 1f);
        float2 axialFrac = math.mul(Board.CartesianToAxial, cartesian).xy;
        Vector2Int axial = new Vector2Int(Mathf.RoundToInt(axialFrac.x), Mathf.RoundToInt(axialFrac.y));
        return AxialToOffset(axial);
    }
    public static Vector3 OffsetToWorld(Vector2Int offsetPos)
    {
        Vector2Int axial = OffsetToAxial(offsetPos);
        float2 cartesian = math.mul(
            Board.AxialToCartesian,
            new float3(axial.x, axial.y, 1f)
        ).xy;
        return new Vector3(cartesian.x, 0f, cartesian.y);
    }
    public static Vector2Int AxialToOffset(Vector2Int axial)
    {
        return new Vector2Int(
            axial.x,
            axial.y + (axial.x - (axial.x & 1)) / 2
        );
    }
    public static Vector2Int OffsetToAxial(Vector2Int offset)
    {
        return new Vector2Int(
            offset.x,
            offset.y - (offset.x - (offset.x & 1)) / 2
        );
    }

    //... Hej Apoorva. Added these because I was going back through Amit's stuff, trying to get my bearings,
    //    and at some point I got bit tired of manually doing the conversion. Normally I myself might make
    //    use of extensions or operator overloading, but if you dislike feel free to chirp up in #tech.
    public static Vector2Int CubicToAxial(Vector3Int cubic)
	{
        return new Vector2Int(cubic.x, cubic.y);
	}
	public static Vector2Int CubicToOffset(Vector3Int cubic)
	{
        Vector2Int axial = Board.CubicToAxial(cubic);
        return Board.AxialToOffset(axial);
	}
	public static Vector3Int OffsetToCubic(Vector2Int offset)
	{
        Vector2Int axial = OffsetToAxial(offset);
        return new Vector3Int(axial.x, axial.y, -axial.x - axial.y);
	}

    //... here I was looking at this neat little trick: https://www.redblobgames.com/grids/hexagons/directions.html
    //... didn't quite get it to work though, think I misunderstood. probably just easier to do some dumb vector math instead
    //... feel free to clear this out, especially if you find a better approach to ClosetCardinalTo.
    public static Vector3Int OffsetToWeird(Vector2Int offset)
    {
        Vector3Int cubicCoord = OffsetToCubic(offset);
        return new Vector3Int(
            cubicCoord.x - cubicCoord.y,
            cubicCoord.y - cubicCoord.z,
            cubicCoord.z - cubicCoord.x
            );
    }
    public static Vector3Int CubicToWeird(Vector3Int cubicCoord)
    {
        return new Vector3Int(
            cubicCoord.x - cubicCoord.y,
            cubicCoord.y - cubicCoord.z,
            cubicCoord.z - cubicCoord.x
            );
    }

    /// Position in offset coordinate space
    public static Vector2Int OffsetPos;

    public static Cell[,] Cells;

    // The assumption here is that there will only ever be a handful of units,
    // so we're not allocating a 2D array of units, one at each position.
    // Instead we're just keeping a 1D array of all units, and to find a unit at
    // a given position, we'll just loop over all of them. A better data
    // structure can be implemented if this ever becomes a perf bottleneck.
    //public static Unit[] Units;

    static List<GridV2> grids = new List<GridV2>();

    static Dictionary<int, Cell> indexToCellLookup = new Dictionary<int, Cell>();

    // Table of neighbor deltas in offset coordinate space
    public static Vector2Int[,] neighborLut = new Vector2Int[,]
    {
        {
            // even cols 
            new Vector2Int(0, +1),  // N
            new Vector2Int(+1, 0),  // NE
            new Vector2Int(+1, -1), // SE
            new Vector2Int(0, -1),  // S
            new Vector2Int(-1, -1), // SW
            new Vector2Int(-1, 0),  // NW
        },
        // odd cols 
        {
            new Vector2Int(0, +1),  // N
            new Vector2Int(+1, +1), // NE
            new Vector2Int(+1, 0),  // SE
            new Vector2Int(0, -1),  // S
            new Vector2Int(-1, 0),  // SW
            new Vector2Int(-1, +1), // NW
        }
    };

    public static Dictionary<HexDirectionFT, Vector2Int> hexDirToStep = new Dictionary<HexDirectionFT, Vector2Int>()
    {
        {HexDirectionFT.N, new Vector2Int(0, 1) },
        {HexDirectionFT.NE, new Vector2Int(1, 0) },
        {HexDirectionFT.SE, new Vector2Int(1, -1) },
        {HexDirectionFT.S, new Vector2Int(0, -1) },
        {HexDirectionFT.SW, new Vector2Int(-1, 0) },
        {HexDirectionFT.NW, new Vector2Int(-1, 1) }
    };


    public static void AddGrid(GridV2 grid)
    {
        bool gridAlreadyAdded = false;
        foreach (GridV2 g in grids)
        {
            if (g.GetInstanceID() == grid.GetInstanceID())
            {
                // ^ The grid is already added 
                gridAlreadyAdded = true;
                break;
            }
        }

        if (gridAlreadyAdded)
        {
            return;
        }

        // ^ The grid is not already added. Add it.
        grids.Add(grid);
    }


    //... RESOURCES:
    static GameObject dampStepSequence;
    public static void Build()
    {
        indexToCellLookup.Clear();

        // Build a list of all cells in the added grids and their min/max bounds
        // in offset coords
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
        var cellsAndCoords = new List<(Cell, Vector2Int)>();

        foreach (var g in grids)
        {
            Cell[] cells = g.GetComponentsInChildren<Cell>();
            cellsAndCoords.Capacity = cellsAndCoords.Count + cells.Length;
            foreach (var cell in cells)
            {
                Vector2Int offset = WorldToOffset(cell.transform.position);

                // Potentially update the bounds
                min = Vector2Int.Min(min, offset);
                max = Vector2Int.Max(max, offset);
                
                // Add the coords and the cell to the list of tuples
                cellsAndCoords.Add((cell, offset));
            }
        }

        Cells = new Cell[max.x - min.x + 1, max.y - min.y + 1];
        foreach (var (cell, coord) in cellsAndCoords)
        {
            int x = coord.x - min.x;
            int y = coord.y - min.y;
            if (Cells[x, y] != null)
            {
                // ^ There is already a cell on this coordinate, which is a
                // mistake. Show an error.
                Debug.LogError($"Duplicate cell found on offset coordinate {coord.x}, {coord.y}.");
            }
            else
            {
                Cells[x, y] = cell;
            }

            int index = coord.ToIndex();
            // ^ this could probably just be using GetInstanceID, but I take it that changes between editor & runtime,
            // which is maybe annoying down the line...? - ian
            if (!indexToCellLookup.TryGetValue(index, out var foundCell))
            {
                indexToCellLookup.Add(index, cell);
			}
			else
			{
                Debug.LogError($"Cell already added to indexLookup at {coord.x}, {coord.y}");
			}
        }

        OffsetPos = min;

        //Units = UnityEngine.Object.FindObjectsOfType<Unit>();
        Debug.Log($"Built a {Cells.GetLength(0)}x{Cells.GetLength(1)}");// board with {Units.Length} units.");

        var ui = BoardUI.FindObjectOfType<BoardUI>();
        Debug.Assert(ui != null, "There is no BoardUI in the scene. Please create one.");
        ui.Init();

        dampStepSequence = Resources.Load(MoveCommand.kDampStepPath) as GameObject;
        currTimeStep = 0;
    }

    public static Unit GetUnitAtPos(Vector2Int pos, bool logDebug = false)
    {
		//Debog.logInput($"checking for unit at: ({pos.ToString()})");

        foreach (Unit unit in Haxan.units)
        {
			//Debog.logInput($"checking {unit.name} at: {unit.OffsetPos.ToString()}");

			if (unit.OffsetPos == pos)
            {
				//Debog.logInput($"... MATCHED {unit.name} at: {unit.OffsetPos.ToString()}");
				// ^ Unit exists at this position
				return unit;
            }
        }
        return null;
    }

    public static bool TryGetCellAtPos(Vector2Int offsetPos, out Cell foundCell)
	{
        foundCell = null;
        if (indexToCellLookup.TryGetValue(offsetPos.ToIndex(), out var cell))
		{
            foundCell = cell;
            return true;
		}

        return false;
    }

    public static Cell TryGetCellAtPos(Vector2Int offsetPos)
	{
        if (indexToCellLookup.TryGetValue(offsetPos.ToIndex(), out var foundCell))
            return foundCell;

        return null;
	}

    public static List<Unit> GetNeighbouringUnits(Vector2Int coord)
	{
        List<Unit> foundUnits = new List<Unit>();

        Vector2Int?[] neighbouringCoords = coord.GetNeighbouringCoords();
        foreach(var neighbouringCoord in neighbouringCoords)
		{
            if (neighbouringCoord == null)
                continue;

            if (!TryGetCellAtPos(neighbouringCoord.Value, out var foundCell))
                continue;

            Unit foundUnit = Board.GetUnitAtPos(neighbouringCoord.Value);
            if (foundUnit == null)
                continue;

            foundUnits.Add(foundUnit);
        }

        return foundUnits;
	}

    // Output positions are in offset coordinates
    public static Vector2Int[] GetNavigableTiles(Unit unit)
    {
        List<Vector2Int> result = new();
        //const int range = 2;
        //const int range = 2;
        // For flying movement:
        /*
        Vector2Int centerAxial = OffsetToAxial(unit.OffsetPos);
        for (int x = -range; x <= range; x++)
        {
            for (int y = Mathf.Max(-range, -x - range); y <= Mathf.Min(range, -x + range); y++)
            {
                // Skip the original positon
                if (x == 0 && y == 0)
                {
                    continue;
                }
                Vector2Int offset = AxialToOffset(centerAxial + new Vector2Int(x, y));
                // Skip this position if it is outside the Board's bounds
                if (offset.x < Board.OffsetPos.x ||
                    offset.x >= Board.OffsetPos.x + Board.Cells.GetLength(0) ||
                    offset.y < Board.OffsetPos.y ||
                    offset.y >= Board.OffsetPos.y + Board.Cells.GetLength(1))
                {
                    continue;
                }
                // Skip this position if it doesn't contain a cell
                if (Board.Cells[offset.x - Board.OffsetPos.x, offset.y - Board.OffsetPos.y] == null)
                {
                    continue;
                }
                result.Add(offset);
            }
        }
        */

        // For ground movement:
        // Perform a breadth-first walk of the cells
        bool[,] visited = new bool[2 * unit.groundedMovementRange + 1, 2 * unit.groundedMovementRange + 1];
        Vector2Int visitedPos = unit.OffsetPos - new Vector2Int(unit.groundedMovementRange, unit.groundedMovementRange);
        List<List<Vector2Int>> fringes = new();

        visited[unit.OffsetPos.x - visitedPos.x, unit.OffsetPos.y - visitedPos.y] = true;
        fringes.Add(new List<Vector2Int>() { unit.OffsetPos });

        for (int i = 0; i < unit.groundedMovementRange; i++)
        {
            fringes.Add(new List<Vector2Int>());
            foreach (var pos in fringes[i])
            {
                int parity = pos.x & 1;
                for (int j = 0; j < 6; j++)
                {
                    Vector2Int neighbor = pos + neighborLut[parity, j];
                    if (visited[neighbor.x - visitedPos.x, neighbor.y - visitedPos.y])
                    {
                        // ^ Already visited. Skip.
                        continue;
                    }

                    if (
                        neighbor.x < Board.OffsetPos.x ||
                        neighbor.x >= Board.OffsetPos.x + Board.Cells.GetLength(0) ||
                        neighbor.y < Board.OffsetPos.y ||
                        neighbor.y >= Board.OffsetPos.y + Board.Cells.GetLength(1)
                        )
                    {
                        // ^ This position is outside the Board's bounds. Skip.
                        continue;
                    }

                    if (Board.Cells[neighbor.x - Board.OffsetPos.x, neighbor.y - Board.OffsetPos.y] == null)
                    {
                        // ^ This position doesn't contain a cell. Skip.
                        continue;
                    }


                    bool cellIsImpassable = false;
                    foreach (var otherUnit in Haxan.units)
                    {
                        if (otherUnit.OffsetPos == neighbor)
                        {
                            if (otherUnit.preset != null && !otherUnit.preset.isPassable)
                                cellIsImpassable = true;
                            else
                                cellIsImpassable = true;
                        }
                    }

                    if (cellIsImpassable)
                        continue;

                    // This cell can be visited.
                    visited[neighbor.x - visitedPos.x, neighbor.y - visitedPos.y] = true;
                    fringes[i + 1].Add(neighbor);
                    result.Add(neighbor);
                }
            }
        }

        return result.ToArray();
    }

	//private static List<Vector2Int> foundPath = new List<Vector2Int>();
	private static List<Cell> exploredCells = new List<Cell>();
	
	private static Dictionary<Cell, float> shortestDistToSource = new Dictionary<Cell, float>();
	
	private static Dictionary<Cell, Cell> prevNodeLookup = new Dictionary<Cell, Cell>();

	public static Vector2Int[] FindPath_NEW(Vector2Int from, Vector2Int to, List<Vector2Int> restrictedCoords = null)
	{
		if (!TryGetCellAtPos(from, out Cell fromCell) || !TryGetCellAtPos(to, out Cell toCell))
			return null;

		Queue<Cell> nodesToCheck = new Queue<Cell>();
		nodesToCheck.Enqueue(fromCell);

		exploredCells.Clear();
		//foundPath.Clear();
		shortestDistToSource.Clear();
		prevNodeLookup.Clear();

		foreach(var kvp in indexToCellLookup)
		{
			shortestDistToSource.Add(kvp.Value, Mathf.Infinity);
			prevNodeLookup.Add(kvp.Value, null);
		}

		shortestDistToSource[fromCell] = 0f;

		while(nodesToCheck.Count > 0)
		{
			var inspectedNode = nodesToCheck.Dequeue();
			exploredCells.Add(inspectedNode);

			if(inspectedNode == toCell)
			{
				var path = new List<Cell>();
				path.Add(toCell);
				
				var backNode = prevNodeLookup[inspectedNode];
				while(backNode != fromCell && backNode != null)
				{
					path.Add(backNode);
					backNode = prevNodeLookup[backNode];
				}

				path.Reverse();
				return path.Select(t => WorldToOffset(t.transform.position)).ToArray();
			}

			var validNeighbours = inspectedNode.transform.position.ToOffset().GetValidNeighbouringCells();

			foreach (var neighbour in validNeighbours)
			{
				//if (
				//	!restrictedCoords.IsNullOrEmpty() 
				//	&& restrictedCoords.Contains(neighbour.transform.position.ToOffset())
				//	)
				//	continue;

				if (exploredCells.Contains(neighbour) || nodesToCheck.Contains(neighbour))
					continue;

				nodesToCheck.Enqueue(neighbour);

				var tentativeDist = shortestDistToSource[inspectedNode];
				tentativeDist += inspectedNode.transform.position.To(neighbour.transform.position).magnitude;

				if(
					shortestDistToSource.ContainsKey(neighbour) 
					&& tentativeDist < shortestDistToSource[neighbour]
					)
				{
					var inspectedNodeToNeighbourDist = inspectedNode.transform.position.To(neighbour.transform.position).magnitude;
					shortestDistToSource[neighbour] = shortestDistToSource[inspectedNode] + inspectedNodeToNeighbourDist;
					prevNodeLookup[neighbour] = inspectedNode;
				}

				prevNodeLookup[neighbour] = inspectedNode;
			}
		}

		return null;
	}

    // Inputs and outputs are in offset coordinates
    public static Vector2Int[] FindPath(Vector2Int src, Vector2Int dst)
    {
        // Since we don't have access to a priority queue, I've just built one
        // out of a List of tuples. The dequeuing is done at the end of the list
        // for efficiency. The priority is high in the beginning of the list,
        // and low at the end.
        List<(Vector2Int, int)> frontier = new();
        frontier.Add((src, 0));

        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        Dictionary<Vector2Int, int> costSoFar = new();
        costSoFar[src] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier[frontier.Count - 1].Item1;
            frontier.RemoveAt(frontier.Count - 1);

            if (current == dst)
            {
                break;
            }

            int parity = current.x & 1;
            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbor = current + neighborLut[parity, i];
                if (
                    neighbor.x < Board.OffsetPos.x ||
                    neighbor.x >= Board.OffsetPos.x + Board.Cells.GetLength(0) ||
                    neighbor.y < Board.OffsetPos.y ||
                    neighbor.y >= Board.OffsetPos.y + Board.Cells.GetLength(1)
                    )
                {
                    // ^ This position is outside the Board's bounds. Skip.
                    continue;
                }
                if (Board.Cells[neighbor.x - Board.OffsetPos.x, neighbor.y - Board.OffsetPos.y] == null)
                {
                    // ^ This position doesn't contain a cell. Skip.
                    continue;
                }

                bool cellIsImpassable = false;
                foreach(var unit in Haxan.units)
				{
                    if (unit.OffsetPos == neighbor)
                        if (unit.preset != null && !unit.preset.isPassable)
                            cellIsImpassable = true;
                        else
                            cellIsImpassable = true;
                }

                if (cellIsImpassable)
                    continue;

				// Currently, movement cost is fixed to 1. If we want variable
				// costs per tile, this is the place to add it.
				int newCost = costSoFar[current] + 1;

                bool contains = costSoFar.TryGetValue(neighbor, out int cost);
                if (!contains || newCost < cost)
                {
                    costSoFar[neighbor] = newCost;
                    // Calculate the heuristic, which is the distance between
                    // the destination and the neighbor
                    int heuristic = 0;
                    {
                        Vector2Int dstAxial = OffsetToAxial(dst);
                        Vector2Int nbrAxial = OffsetToAxial(neighbor);
                        Vector2Int delta = dstAxial - nbrAxial;
                        heuristic = (Mathf.Abs(delta.x) + Mathf.Abs(delta.x + delta.y) + Mathf.Abs(delta.y)) / 2;
                    }
                    int priority = newCost + heuristic;
                    // Insert the neighbor into our makeshift priority queue
                    if (frontier.Count == 0)
                    {
                        // ^ The priority queue is empty
                        frontier.Add((neighbor, priority));
                    }
                    else if (frontier[frontier.Count - 1].Item2 > priority)
                    {
                        // ^ The lowest priority in the queue is still higher
                        // than the one we want to insert. Make our item the
                        // last one, so that it will be dequeued first.
                        frontier.Add((neighbor, priority));
                    }
                    else
                    {

                        int idx = 0;
                        for (int j = 0; j < frontier.Count; j++)
                        {
                            if (frontier[j].Item2 <= priority)
                            {
                                break;
                            }
                        }
                        frontier.Insert(idx, (neighbor, priority));
                    }

                    cameFrom[neighbor] = current;
                }
            }
        }

        List<Vector2Int> path = new();
        {
            Vector2Int current = dst;
            while (current != src)
            {
                path.Add(current);
                current = cameFrom[current];
            }
            path.Reverse();
        }

        return path.ToArray();
    }

    //... TODO: maybe something more continuous could be done.
    //... every command gets its own little authored timings
    //...   ie. at 0.68f seconds of normalized time, do a splash.
    public static void ProcessCommandFrame(Unit unit, UnitCommand command)
	{
	}

    public static void RespondToCommandBeginTick(Unit unit, UnitCommand command)
	{

	}

    public static void RespondToCommandCompleteTick(Unit unit, UnitCommand command)
	{
        //if (UnitCommandTimeline.I == null)
        //    return;

        Debug.LogWarning("responding to command tick complete");
        if (command is MoveCommand)
        {
            //... ^^ run splashes, we're touching down in the "to" coord here.
            MoveCommand stepCommand = command as MoveCommand;
			if (TryGetCellAtPos(stepCommand.toCoord, out var foundCell))
			{
				if ((foundCell.surfaceFlags & CellSurfaceFlags.DAMP) == CellSurfaceFlags.DAMP)
				{
					Debug.LogWarning($"cell at {stepCommand.toCoord} was damp.");

                    UnitCommandTimeline.I.StartSequence(unit, dampStepSequence, stepCommand.toCoord, currTimeStep);
                    //TimeStepSequence newDampStepSeq = GameObject.Instantiate(
                    //    dampStepSequence,
                    //    //true,
                    //    OffsetToWorld(stepCommand.toCoord),
                    //    Quaternion.identity,
                    //    UnitCommandTimeline.I.transform
                    //    ).GetComponent<TimeStepSequence>();
				}
			}
        }
    }

    public static void OnUnitEnteredCell(Unit unit, Vector2Int coord) 
    { 
    
    }

    public static void OnUnitExitedCell(Unit unit, Vector2Int coord) 
    { 
    
    }
}

public static class BoardExtensions
{
    public static Vector2Int?[] GetNeighbouringCoords(this Vector2Int coord)
	{
        //CellV2_NEW[] neighbs = new CellV2_NEW[6];
        Vector2Int?[] neighbours = new Vector2Int?[6];

        //Debug.LogWarning("checking neighbs at: " + coord.ToString());

        int parity = coord.x & 1;
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbour = coord + Board.neighborLut[parity, i];

            //Debug.LogWarning("checking neighb at: " + ((HexDirectionFT)i).ToString());

            if (
                neighbour.x < Board.OffsetPos.x ||
                neighbour.x >= Board.OffsetPos.x + Board.Cells.GetLength(0) ||
                neighbour.y < Board.OffsetPos.y ||
                neighbour.y >= Board.OffsetPos.y + Board.Cells.GetLength(1)
                )
            {
                neighbours[i] = null;
                // ^ This position is outside the Board's bounds. Skip.
                continue;
            }

			//neighbours[i] = Board.Cells[neighbour.x - Board.OffsetPos.x, neighbour.y - Board.OffsetPos.y];

			if (Board.Cells[neighbour.x - Board.OffsetPos.x, neighbour.y - Board.OffsetPos.y] == null)
			{
                //Debug.LogWarning("")
				neighbours[i] = null;
				// ^ This position doesn't contain a cell. Skip.
				continue;
			}

            //Debug.LogWarning("... neighb to the: " + ((HexDirectionFT)i).ToString() + " " + neighbour.ToString());

            neighbours[i] = neighbour;
		}

        return neighbours;
    }

    public static bool IsNeighbourOf(this Vector2Int coord, Vector2Int targetCoord)
	{
        int parity = coord.x & 1;
        Vector2Int delta = targetCoord - coord;
        int j;
        for(j = 0; j < 6; j++)
		{
            if (Board.neighborLut[parity, j] == delta)
                return true;
		}

        return false;
	}

    public static Cell[] GetNeighbouringCells(this Vector2Int coord)
    {
        Cell[] neighbs = new Cell[6];
        //List<Vector2Int> neighbours = new List<Vector2Int>();

        int parity = coord.x & 1;
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbour = coord + Board.neighborLut[parity, i];

            Debug.LogWarning("checking neighb at: " + ((HexDirectionFT)i).ToString());

            if (
                neighbour.x < Board.OffsetPos.x ||
                neighbour.x >= Board.OffsetPos.x + Board.Cells.GetLength(0) ||
                neighbour.y < Board.OffsetPos.y ||
                neighbour.y >= Board.OffsetPos.y + Board.Cells.GetLength(1)
                )
            {
                neighbs[i] = null;
                // ^ This position is outside the Board's bounds. Skip.
                continue;
            }

            neighbs[i] = Board.Cells[neighbour.x - Board.OffsetPos.x, neighbour.y - Board.OffsetPos.y];

            //if (Board.Cells[neighbour.x - Board.OffsetPos.x, neighbour.y - Board.OffsetPos.y] == null)
            //{
            //    neighbs[i] = null;
            //    // ^ This position doesn't contain a cell. Skip.
            //    continue;
            //}

            //neighbs[i] = 

            //neighbours.Add(neighbour);
        }

        return neighbs;
    }

	public static List<Vector2Int> GetValidNeighbouringCoords(this Vector2Int coord)
	{
		var neighbours = coord.GetNeighbouringCoords();
		var validNeighbours = new List<Vector2Int>();
		foreach(var neighbour in neighbours)
		{
			if (neighbour == null)
				continue;

			if (neighbour.TryGetUnitAtCoord(out var foundUnit))
			{
				if (!foundUnit.preset.isPassable)
					continue;
			}

			validNeighbours.Add((Vector2Int)neighbour);
		}

		return validNeighbours;
	}

	public static List<Cell> GetValidNeighbouringCells(this Vector2Int coord)
	{
		var neighbours = coord.GetNeighbouringCoords();
		var validNeighbours = new List<Cell>();
		foreach (var neighbour in neighbours)
		{
			if (neighbour == null)
				continue;

			if (Board.TryGetCellAtPos(neighbour.Value, out Cell foundCell))
			{
				if(neighbour.TryGetUnitAtCoord(out var foundUnit))
				{
					if (!foundUnit.preset.isPassable)
						continue;
				}

				validNeighbours.Add(foundCell);
			}
		}

		return validNeighbours;
	}

    public static HexDirectionFT ToNeighbour(this Vector2Int from, Vector2Int to)
	{
        if (!from.IsNeighbourOf(to))
            return (HexDirectionFT)(-1);

        int parity = from.x & 1;
        Vector2Int delta = to - from;
        int i;
		for (i = 0; i < 6; i++)
		{
            if (Board.neighborLut[parity, i] == delta)
                break;
		}

        return (HexDirectionFT)i;
    }

    public static int ClockwiseTo(this HexDirectionFT fromDir, HexDirectionFT toDir)
	{
        if (fromDir == toDir)
            return 0;

        int increment = 0;
        int tempDir = (int)fromDir;

		for (int i = 0; i < 6; i++)
		{
            increment++;
            tempDir++;

            if (tempDir >= 6)
                tempDir = 0;

            if ((HexDirectionFT)tempDir == toDir)
                return increment;
		}

        return -1;

  //      if (from == to)
  //          return 0;

  //      if((6 - (int)to) < (to - from))
		//{
  //          return (6 - (int)to) + (int)from;
		//}

  //      if((6 - (int)to) > (to - from))
		//{
  //          return to - from;
		//}

  //      return 3;
	}

    public static int CounterClockwiseTo(this HexDirectionFT fromDir, HexDirectionFT toDir)
    {
        if (fromDir == toDir)
            return 0;

        int increment = 0;
        int tempDir = (int)fromDir;

        for (int i = 0; i < 6; i++)
        {
            increment++;
            tempDir--;

            if (tempDir < 0)
                tempDir = 5;

            if ((HexDirectionFT)tempDir == toDir)
                return increment;
        }

        return -1;

        //if ((int)to < (from - to))
        //{
        //    return (6 - (int)to) + (int)from;
        //}

        //if ((6 - (int)to) > (to - from))
        //{
        //    return to - from;
        //}

        //return 3;
    }

    public static void SnapToGrid(this Transform transform)
	{
        float3 cartesian = new float3(transform.position.x, transform.position.z, 1f);
        float2 axial = math.mul(Board.CartesianToAxial, cartesian).xy;
        axial = math.round(axial);
        float2 roundedCartesian = math.mul(Board.AxialToCartesian, new float3(axial.x, axial.y, 1f)).xy;

        Vector3 pos = new Vector3(
            roundedCartesian.x,
            0f,
            roundedCartesian.y
        );

        transform.position = pos;
        transform.hasChanged = false;
    }

    public static int ToIndex(this Vector2Int pos) => pos.x + pos.y * 10000;

    public static bool IsOccupied(this Vector2Int coord)
	{
        foreach (var unit in Haxan.units)
        {
            if (unit.OffsetPos == coord)
                return true;

                //if (unit.preset != null && !unit.preset.isPassable)
                //    cellIsImpassable = true;
                //else
                //    cellIsImpassable = true;
        }

        return false;
    }

	public static bool TryGetUnitAtCoord(this Vector2Int? coord, out Unit foundUnit)
	{
		if (coord == null)
		{
			foundUnit = null;
			return false;
		}

		Vector2Int nonNullableCoord = (Vector2Int)coord; 

		return nonNullableCoord.TryGetUnitAtCoord(out foundUnit);
	}

    public static bool TryGetUnitAtCoord(this Vector2Int coord, out Unit foundUnit)
	{
        foundUnit = null;
        Unit unit = Board.GetUnitAtPos(coord);
        if(unit != null)
		{
            foundUnit = unit;
            return true;
		}
        return false;
	}
}