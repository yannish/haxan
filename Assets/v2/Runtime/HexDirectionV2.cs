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
		if(radius == 0)
		{
			offsetCoords.Add(originOffsetCoord);
			return offsetCoords;
		}

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

		//if (radius == 1)
		//	return GetCellsInRadius(originOffsetCoord, radius);

		var cellsInInnerRadius = GetCellsInRadius(originOffsetCoord, radius - 1);
		var cellsInOuterRadius = GetCellsInRadius(
			originOffsetCoord, 
			radius, 
			(Vector2Int coord) => !cellsInInnerRadius.Contains(coord)
			);

		return cellsInOuterRadius;
	}

	public static List<Vector2Int> GetCardinalLine(
		this Vector2Int originCoord, 
		HexDirectionFT dir, 
		int length, 
		int min = 0, 
		HexOcclusion occlusionType = HexOcclusion.NONE, 
		Predicate<Vector2Int> check = null
		)
	{
		var coords = new List<Vector2Int>();

		for (int i = min; i <= length; i++)
		{
			var coord = originCoord.Step(dir, i);

			if (Board.TryGetCellAtPos(coord))
			{
				if(check != null && check.Invoke(coord))
				{
					if (occlusionType == HexOcclusion.NONE)
						continue;

					//... if you're running w/ excl occlusion, we DON'T include the failed cell:
					else if (occlusionType == HexOcclusion.EXCLUSIVE)
						break;
					else
					{
						//... if you're running w/ incl occlusion, we DO include the failed cell:
						coords.Add(coord);
						break;
					}
				}
				else
				{
					coords.Add(coord);
				}
			}
		}

		return coords;
	}

	public static List<Vector2Int> GetCardinalCross(
		this Vector2Int originCoord,
		int radius,
		int min = 0,
		HexOcclusion occlusionType = HexOcclusion.NONE,
		Predicate<Vector2Int> check = null
		)
	{
		var coords = new List<Vector2Int>();

		for (HexDirectionFT dir = HexDirectionFT.N; dir <= HexDirectionFT.NW; dir++)
		{
			coords.AddRange(GetCardinalLine(originCoord, dir, radius, min, occlusionType, check));
		}

		return coords;
	}

	public static Vector2Int Step(this Vector2Int offsetCoord, HexDirectionFT direction, int distance)
	{
		if (distance == 0)
			return offsetCoord;

		Vector2Int axialCoord = Board.OffsetToAxial(offsetCoord);
		Vector2Int steppedCoord = axialCoord;

		Vector2Int axialStep = Board.hexDirToStep[direction];

		return Board.AxialToOffset(axialCoord + axialStep * distance);

		//switch (direction)
		//{
		//	case HexDirectionFT.N:
		//		steppedCoord = new Vector2Int(axialCoord.x, axialCoord.y - distance);
		//		break;
		//	case HexDirectionFT.NE:
		//		steppedCoord = new Vector2Int(axialCoord.x + distance, axialCoord.y - distance);
		//		break;
		//	case HexDirectionFT.SE:
		//		steppedCoord = new Vector2Int(axialCoord.x + distance, axialCoord.y);
		//		break;
		//	case HexDirectionFT.S:
		//		steppedCoord = new Vector2Int(axialCoord.x, axialCoord.y + distance);
		//		break;
		//	case HexDirectionFT.SW:
		//		steppedCoord = new Vector2Int(axialCoord.x - distance, axialCoord.y + distance);
		//		break;
		//	case HexDirectionFT.NW:
		//		steppedCoord = new Vector2Int(axialCoord.x - distance, axialCoord.y);
		//		break;
		//}

		//return Board.AxialToOffset(steppedCoord);
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
