using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Board
{
    public static readonly float3x3 CartesianToAxial = math.mul(
        new float3x3(
            2f / (3f * CellV2.OuterRadius), 0f, 0f,
            -1f / (3f * CellV2.OuterRadius), Mathf.Sqrt(3f) / (3f * CellV2.OuterRadius), 0f,
            0f, 0f, 1f
        ),
        new float3x3(
            1f, 0f, -CellV2.OuterRadius,
            0f, 1f, -CellV2.InnerRadius,
            0f, 0f, 1f
        )
    );
    public static readonly float3x3 AxialToCartesian = math.mul(
        new float3x3(
            1f, 0f, CellV2.OuterRadius,
            0f, 1f, CellV2.InnerRadius,
            0f, 0f, 1f
        ),
        new float3x3(
            1.5f * CellV2.OuterRadius, 0f, 0f,
            CellV2.InnerRadius, CellV2.InnerRadius * 2f, 0f,
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

    /// Position in offset coordinate space
    public static Vector2Int OffsetPos;
    public static CellV2[,] Cells;
    // The assumption here is that there will only ever be a handful of units,
    // so we're not allocating a 2D array of units, one at each position.
    // Instead we're just keeping a 1D array of all units, and to find a unit at
    // a given position, we'll just loop over all of them. A better data
    // structure can be implemented if this ever becomes a perf bottleneck.
    public static Unit[] Units;

    static List<Grid> grids = new List<Grid>();

    public static void AddGrid(Grid grid)
    {
        bool gridAlreadyAdded = false;
        foreach (Grid g in grids)
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

    public static void Build()
    {
        // Build a list of all cells in the added grids and their min/max bounds
        // in offset coords
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
        var cellsAndCoords = new List<(CellV2, Vector2Int)>();
        foreach (var g in grids)
        {
            CellV2[] cells = g.GetComponentsInChildren<CellV2>();
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

        Cells = new CellV2[max.x - min.x + 1, max.y - min.y + 1];
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
        }

        OffsetPos = min;

        Units = Object.FindObjectsOfType<Unit>();
        Debug.Log($"Built a {Cells.GetLength(0)}x{Cells.GetLength(1)} board with {Units.Length} units.");

        var ui = BoardUI.FindObjectOfType<BoardUI>();
        Debug.Assert(ui != null, "There is no BoardUI in the scene. Please create one.");
        ui.Init();
    }

    public static Unit GetUnitAtPos(Vector2Int pos)
    {
        foreach (Unit unit in Board.Units)
        {
            if (unit.OffsetPos == pos)
            {
                // ^ Unit exists at this position
                return unit;
            }
        }
        return null;
    }
    // Output positions are in offset coordinates
    public static Vector2Int[] GetNavigableTiles(Unit unit)
    {
        List<Vector2Int> result = new();
        const int range = 3;
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
        return result.ToArray();
    }
}
