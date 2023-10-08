using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
	NE, E, SE, SW, W, NW
}

public enum HexDirectionFT
{
	N, NE, SE, S, SW, NW
}

public enum HexOcclusion
{
	NONE,
	EXCLUSIVE,
	INCLUSIVE
}

public static class HexDirectionV2
{
	public static Dictionary<HexDirection, float> HexPTAngleLookup = new Dictionary<HexDirection, float>()
	{
		{HexDirection.NE, 30f },
		{HexDirection.E, 90f },
		{HexDirection.SE, 150f },
		{HexDirection.SW, 210f },
		{HexDirection.W, 270f },
		{HexDirection.NW, 330f }
	};

	public static Dictionary<HexDirectionFT, float> HexFTAngleLookup = new Dictionary<HexDirectionFT, float>()
	{
		{HexDirectionFT.N, 0f },
		{HexDirectionFT.NE, 60f },
		{HexDirectionFT.SE, 120f },
		{HexDirectionFT.S, 180f },
		{HexDirectionFT.SW, 240f },
		{HexDirectionFT.NW, 300f }
	};

	public static float ToAngle(this HexDirection direction) => HexPTAngleLookup[direction];

	public static float ToAngle(this HexDirectionFT direction) => HexFTAngleLookup[direction];

	public static Vector3 ToVector(this HexDirection direction) => Quaternion.AngleAxis(HexPTAngleLookup[direction], Vector3.up) * Vector3.forward;

	public static Vector3 ToVector(this HexDirectionFT dir) => Quaternion.AngleAxis(HexFTAngleLookup[dir], Vector3.up) * Vector3.forward;

	private static Vector3[] _VectorHexDirections;

	public static Vector3[] VectorHexDirections
	{
		get
		{
			if (_VectorHexDirections.IsNullOrEmpty())
			{
				Vector3[] directions = new Vector3[6];
				int i = 0;
				foreach (HexDirection dir in HexPTAngleLookup.Keys)
				{
					var angle = HexPTAngleLookup[dir];
					var rot = Quaternion.AngleAxis(angle, Vector3.up);
					directions[i] = rot * Vector3.forward;
					i++;
				}
				_VectorHexDirections = directions;
			}
			return _VectorHexDirections;
		}
	}

	public static HexDirection Opposite(this HexDirection direction)
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}

	public static HexDirectionFT Opposite(this HexDirectionFT direction)
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}

	public static HexDirection Rotate(this HexDirection direction, int steps)
	{
		return (HexDirection)(((int)direction + steps) % 6);
	}

	public static HexDirectionFT Rotate(this HexDirectionFT direction, int steps)
	{
		return (HexDirectionFT)(((int)direction + steps) % 6);
	}

	public static HexDirection Previous(this HexDirection direction)
	{
		return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
	}

	public static HexDirectionFT Previous(this HexDirectionFT direction)
	{
		return direction == HexDirectionFT.N ? HexDirectionFT.NW : (direction - 1);
	}

	public static HexDirection Next(this HexDirection direction)
	{
		return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
	}

	public static HexDirectionFT Next(this HexDirectionFT direction)
	{
		return direction == HexDirectionFT.NW ? HexDirectionFT.N : (direction + 1);
	}

	public static List<Vector2Int> GetSwingPath(
		this Vector2Int originCoord,
		Vector2Int startCoord,
		bool counterClockwise = false,
		int arcLength = 5
		)
	{
		if (!startCoord.IsNeighbourOf(originCoord))
		{
			Debug.LogWarning("swinging coord isn't a neighbour of origin!");
			return null;
		}

		List<Vector2Int> swingPath = new List<Vector2Int>();
		swingPath.Add(startCoord);

		Vector2Int currCoord = startCoord;
		HexDirectionFT currDir;

		for (int i = 0; i < arcLength; i++)
		{
			currDir = counterClockwise ?
				originCoord.ToNeighbour(currCoord).Previous() :
				originCoord.ToNeighbour(currCoord).Next();

			currCoord = originCoord.Step(currDir, 1);

			swingPath.Add(currCoord);
		}

		return swingPath;
	}

	public static List<Vector2Int> GetCellsInSwing(
		this Vector2Int originCoord,
		Vector2Int swungCoord,
		bool counterClockwise
		)
	{
		if(!swungCoord.IsNeighbourOf(originCoord))
		{
			Debug.LogWarning("swinging coord isn't a neighbour of origin!");
			return null;
		}

		List<Vector2Int> arcPath = new List<Vector2Int>(); 
		List<Vector2Int> swingPath = originCoord.GetSwingPath(swungCoord);

		List<Vector2Int> cells = new List<Vector2Int>();
		cells.Add(originCoord);

		HexDirectionFT originToSwung = originCoord.ToNeighbour(swungCoord);
		int i = 0;
		bool hitUnit = false;
		if (!counterClockwise)
		{
			HexDirectionFT nextDir = originToSwung.Next();

			//... swing around 5 cells clockwise:
			while(i < 5 && !hitUnit)
			{

			}

			Vector2Int nextCoord = swungCoord.Step(nextDir, 1);
			if (Board.TryGetCellAtPos(nextCoord, out var foundCell))
			{
				Unit foundUnit = Board.GetUnitAtPos(nextCoord);
				if(foundUnit != null)
				{

				}
			}
			else
			{

			}
			//if(nextCoord)
		}
		return cells;
	}

	public static List<Vector2Int> GetCellsInRadius(
		this Vector2Int originOffsetCoord,
		int radius,
		Predicate<Vector2Int> check = null,
		bool includeOrigin = false
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

				if (!includeOrigin && offsetCoord == originOffsetCoord)
					continue;

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
