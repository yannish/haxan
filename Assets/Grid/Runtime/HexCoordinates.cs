using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoordinateExtensions
{
	public static OffsetCoordinates ToOffsetCoords(this HexCoordinates hexCoords)
	{
		int col = hexCoords.X + (hexCoords.Z - (hexCoords.Z & 1)) / 2;
		int row = hexCoords.Z;
		return new OffsetCoordinates(col, row);
	}


	public static HexCoordinates ToCubeCoords(this OffsetCoordinates offsetCoords)
	{
		int X = offsetCoords.column - (offsetCoords.row - offsetCoords.row & 1) / 2;
		int Z = offsetCoords.row;
		return new HexCoordinates(X, Z);
	}
}

[Serializable]
public struct OffsetCoordinates
{
	//[SerializeField]
	public int column;
	//[SerializeField]
	public int row;

	public OffsetCoordinates(int x, int z)
	{
		this.column = x;
		this.row = z;
	}

	public override string ToString()
	{
		return "(" + column.ToString() + ", " + row.ToString() + ")";
	}

	//	function cube_to_oddr(cube):

	//	var col = cube.x + (cube.z - (cube.z & 1)) / 2
	//	var row = cube.z
	//    return OffsetCoord(col, row)

	//function oddr_to_cube(hex):

	//	var x = hex.col - (hex.row - (hex.row & 1)) / 2
	//	var z = hex.row
	//	var y = -x - z
	//    return Cube(x, y, z)
}

[Serializable]
public struct HexCoordinates
{
	[SerializeField]
	private int x;
	[SerializeField]
	private int z;

	public int X { get { return x; } }
	public int Z { get { return z; } }
	public int Y { get { return - X - Z; } }

	public HexCoordinates(int x, int z)
	{
		this.x = x;
		this.z = z;
	}

	public Vector3 WorldPos()
	{
		return new Vector3(
			(x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2f,
			0f,
			z * (HexMetrics.outerRadius * 1.5f)
			);
	}

	public static HexCoordinates FromOffsetCoordinates(int x, int z)
	{
		return new HexCoordinates(x - z / 2, z);
	}

	public override string ToString()
	{
		return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}
}
