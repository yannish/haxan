using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexDirectionV2
{
	public static List<Vector2Int> GetCellsInRadius(
		this Vector2Int originOffsetCoord,
		int radius,
		Predicate<Vector2Int> check = null
		)
	{
		List<Vector2Int> offsetCoords = new List<Vector2Int>();

		Vector3Int originCubicCoord = Board.OffsetToCubic(originOffsetCoord);

		int xMin = originCubicCoord.x - radius;
		int xMax = originCubicCoord.x + radius;

		int yMin = originCubicCoord.y - radius;
		int yMax = originCubicCoord.y + radius;

		int zMin = originCubicCoord.z - radius;
		int zMax = originCubicCoord.z + radius;

		for (int x = xMin; x <= xMax; x++)
		{
			for (int y = Mathf.Max(yMin, -x - zMax); y <= Mathf.Min(yMax, -x - zMin); y++)
			{
				Vector2Int axial = new Vector2Int(x, y);
				Vector2Int offsetCoord = Board.AxialToOffset(axial);

				if (Board.TryGetCellAtPos(offsetCoord))
				{
					if (check != null && !check(offsetCoord))
						continue;
					offsetCoords.Add(offsetCoord);
				}
			}
		}

		return offsetCoords;
	}

	public static List<Vector2Int> GetCardinalRing(this Vector2Int originOffsetCoord, int radius)
	{
		if (radius == 0)
			return null;

		if (radius == 1)
			return GetCellsInRadius(originOffsetCoord, radius);

		var cellsInInnerRadius = GetCellsInRadius(originOffsetCoord, radius - 1);
		var cellsInOuterRadius = GetCellsInRadius(
			originOffsetCoord, 
			radius, 
			(Vector2Int coord) => !cellsInInnerRadius.Contains(coord)
			);

		return cellsInOuterRadius;
	}

	public static HexDirectionFT ClosetCardinalTo(this Vector2Int originOffsetCoord, Vector2Int targetOffsetCoord)
	{
		HexDirectionFT returnDir = HexDirectionFT.N;

		Vector3Int cubicFrom = Board.OffsetToCubic(originOffsetCoord);
		Vector3Int cubicTo = Board.OffsetToCubic(targetOffsetCoord);

		Vector3Int deltaCubic = cubicTo - cubicFrom;
		//Vector3Int deltaCubic = Board.OffsetToCubic(offsetCoord - targetCoord);

		Debug.LogWarning("... delta: " + deltaCubic);

		Vector3Int weirdCubic = Board.CubicToWeird(deltaCubic);

		Debug.LogWarning("... weird: " + weirdCubic);

		var deltaXY = Mathf.Abs(weirdCubic.y - weirdCubic.x);
		var deltaYZ = Mathf.Abs(weirdCubic.z - weirdCubic.y);
		var deltaXZ = Mathf.Abs(weirdCubic.x - weirdCubic.z);

		//var max = MoreMath.Max(deltaXY, deltaYZ, deltaXZ);

		if (deltaXY >= deltaXZ && deltaXY >= deltaYZ)
		{
			if (weirdCubic.z > 0)
				returnDir = HexDirectionFT.NW;
			else
				returnDir = HexDirectionFT.SE;
		}

		if (deltaXZ >= deltaXY && deltaXZ >= deltaYZ)
		{
			if (weirdCubic.x > 0)
				returnDir = HexDirectionFT.NE;
			else
				returnDir = HexDirectionFT.SW;
		}

		if (deltaYZ >= deltaXZ && deltaYZ >= deltaXY)
		{
			if (weirdCubic.y > 0)
				returnDir = HexDirectionFT.S;
			else
				returnDir = HexDirectionFT.N;
		}

		return returnDir;
	}
}
