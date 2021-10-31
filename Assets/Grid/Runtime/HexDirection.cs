using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0649 // private field assigned but not used.


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

public static class HexDirectionExtensions
{
    public static float ToAngle(this HexDirection direction)
	{
		return HexPTAngleLookup[direction];
	}

	public static HexDirection ClosestCardinal(this Vector3 vector)
	{
		var flatVector = vector.FlatInXZ();

		float closestDot = -2f;
		HexDirection closestCardinal = 0;
		for (int i = 0; i < VectorHexDirections.Length; i++)
		{
			var score = Vector3.Dot(VectorHexDirections[i], vector);
			if (score > closestDot)
			{
				closestDot = score;
				closestCardinal = (HexDirection)i;
			}
		}

		return closestCardinal;
	}

	public static Vector3 ToVector(this HexDirection direction)
	{
		return Quaternion.AngleAxis(HexPTAngleLookup[direction], Vector3.up) * Vector3.forward;
		//return Quaternion.AngleAxis(30f + 60f * (int)direction, Vector3.up) * Vector3.forward;
	}


	private static Vector3[] _VectorHexDirections;
	public static Vector3[] VectorHexDirections
	{
		get
		{
			if(_VectorHexDirections.IsNullOrEmpty())
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

	public static HexDirection Opposite(this HexDirection direction)
	{
		return (int) direction < 3 ? (direction + 3) : (direction - 3);
	}

	public static HexDirectionFT Opposite(this HexDirectionFT direction)
	{
		return (int) direction < 3 ? (direction + 3) : (direction - 3);
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


	public static HexDirection To(this Cell from, Cell to)
	{
		return (HexDirection)Array.IndexOf(from.neighbours, to);
	}



	public static int StepsTo(this HexDirection direction, HexDirection targetDirection)
	{
		var rtrn = Mathf.Min(
			Mathf.Abs(direction - targetDirection),
			Mathf.Abs(6 - Mathf.Abs(direction - targetDirection))
			);

		return rtrn;
	}
}

public static class HexCoordinateExtensions
{
	public static HexCoordinates Step(this HexCoordinates coord, HexDirection direction, int distance)
	{
		HexCoordinates steppedCoord = coord;
		if (distance == 0)
			return steppedCoord;

		switch(direction)
		{
			case HexDirection.NE:
				steppedCoord = new HexCoordinates(coord.X, coord.Z + distance);
				break;
			case HexDirection.E:
				steppedCoord = new HexCoordinates(coord.X + distance, coord.Z);
				break;
			case HexDirection.SE:
				steppedCoord = new HexCoordinates(coord.X + distance, coord.Z - distance);
				break;
			case HexDirection.SW:
				steppedCoord = new HexCoordinates(coord.X, coord.Z - distance);
				break;
			case HexDirection.W:
				steppedCoord = new HexCoordinates(coord.X - distance, coord.Z);
				break;
			case HexDirection.NW:
				steppedCoord = new HexCoordinates(coord.X - distance, coord.Z + distance);
				break;
		}

		return steppedCoord;
	}

	//public static HexDirection CardinalTo(this HexCell fromCell, HexCell toCell)
	//{
	//	HexDirection direction = fromCell.ClosestCardinalTo(toCell);
	//	int distance = fromCell.CubeDistance(toCell);
	//	List<HexCell> grabbedCells = fromCell.coords.GetCardinalLine(direction, distance, distance - 1);

	//	return grabbedCells.Contains(toCell) ? direction : (HexDirection)(-1);
	//}

	public static HexDirection ClosestCardinalTo(this Cell cell, Cell targetCell)
	{
		var deltaCoord = new HexCoordinates(
			cell.coords.X - targetCell.coords.X,
			cell.coords.Z - targetCell.coords.Z
			);

		var deltaXY = Mathf.Abs(deltaCoord.X - deltaCoord.Y);
		var deltaYZ = Mathf.Abs(deltaCoord.Y - deltaCoord.Z);
		var deltaXZ = Mathf.Abs(deltaCoord.Z - deltaCoord.X);

		var max = MoreMath.Max(deltaXY, deltaXZ, deltaYZ);

		HexDirection returnDir = HexDirection.E;

		if (deltaXY >= deltaXZ && deltaXY >= deltaYZ)
		{
			if (deltaCoord.X < 0)
				returnDir = HexDirection.W;
			else
				returnDir = HexDirection.E;
		}

		if (deltaXZ >= deltaXY && deltaXZ >= deltaYZ)
		{
			if (deltaCoord.Z > 0)
				returnDir = HexDirection.NW;
			else
				returnDir = HexDirection.SE;
		}

		if (deltaYZ >= deltaXZ && deltaYZ >= deltaXY)
		{
			if (deltaCoord.Y > 0)
				returnDir = HexDirection.SW;
			else
				returnDir = HexDirection.NE;
		}

		return returnDir;
	}

	public static List<Cell> GetCardinalCross(
		this HexCoordinates start,
		int length,
		int min = 0, 
		HexOcclusion occlusionType = HexOcclusion.NONE,
		Predicate<Cell> check = null
		)
	{
		var cells = new List<Cell>();

		for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
		{
			var currLine = GetCardinalLine(start, dir, length, min, occlusionType, check);
			cells.AddRange(currLine);
		}

		return cells;
	}

	public static List<Cell> GetCardinalArc(
		this Cell cell,
		HexDirection direction,
		int radius,
		Predicate<Cell> check = null
		)
	{
		var cells = new List<Cell>();

		//HexCoordinates frontCoord = cell.coords.Step(direction, radius);
		//HexCoordinates leftCoord = cell.coords.Step(direction.Previous(), radius);
		//HexCoordinates rightCoord = cell.coords.Step(direction.Next(), radius);

		//if (Globals.Grid.coordCellLookup.TryGetValue(frontCoord, out HexCell foundCell))
		//	if (
		//		check == null
		//		|| check != null && check.Invoke(foundCell)
		//		)
		//		cells.Add(foundCell);

		//if (Globals.Grid.coordCellLookup.TryGetValue(leftCoord, out HexCell foundCell))
		//	if (
		//		check == null
		//		|| check != null && check.Invoke(foundCell)
		//		)
		//		cells.Add(foundCell);

		//if (Globals.Grid.coordCellLookup.TryGetValue(rightCoord, out HexCell foundCell))
		//	if (
		//		check == null
		//		|| check != null && check.Invoke(foundCell)
		//		)
		//		cells.Add(foundCell);

		return cells;
	}

	public static List<Cell> GetCardinalRing(
		this Cell cell,
		int radius,
		Predicate<Cell> check = null
		)
	{
		var cells = new List<Cell>();

		for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
		{
			var startCoord = cell.coords.Step(dir, radius);
			cells.AddRange(
				GetCardinalLine(
					startCoord, 
					dir.Previous().Previous(), 
					radius - 1,
					check : check
					)
				);
		}

		return cells;
	}

	public static List<Cell> GetCardinalLine(
		this HexCoordinates startCoords,
		HexDirection direction,
		int length,
		int min = 0,
		HexOcclusion occlusionType = HexOcclusion.NONE,
		Predicate<Cell> check = null
		)
	{
		var cells = new List<Cell>();

		for (int i = min; i <= length; i++)
		{
			var coord = startCoords.Step(direction, i);

			//Debug.Log("Checking coord:" + coord.ToString());
			//Debug.Log("... lookup had : " + Globals.Grid.coordCellLookup.Count);

			//... if there's an actual cell at this coordinate:
			if (Globals.Grid.coordCellLookup.TryGetValue(coord, out Cell foundCell))
			{
				if (foundCell)
				{
					//... if you have a check, run it:
					if (check != null && !check.Invoke(foundCell))
					{
						if (occlusionType == HexOcclusion.NONE)
							continue;
						
						//... if you're running w/ excl occlusion, we DON'T include the failed cell:
						else if (occlusionType == HexOcclusion.EXCLUSIVE)
							break;
						else
						{
							//... if you're running w/ incl occlusion, we DO include the failed cell:
							cells.Add(foundCell);
							break;
						}
					}
					else
					{
						cells.Add(foundCell);
					}
				}
			}
		}

		return cells;
	}

	public static List<Cell> GetCellsInRadius(
		this Cell cell,
		int radius,
		Predicate<Cell> check = null
		)
	{
		if (radius < 1) 
			return null;

		//var cell = Globals.Selector.CheckGrid(cell.transform.position);
		var grabbedCells = new List<Cell>();

		//if (radius == 1) { grabbedCells.Add(cell); return grabbedCells; }

		int xMin = cell.coords.X - radius;
		int xMax = cell.coords.X + radius;
		int zMin = cell.coords.Z - radius;
		int zMax = cell.coords.Z + radius;
		int yMin = cell.coords.Y - radius;
		int yMax = cell.coords.Y + radius;

		for (int x = xMin; x <= xMax; x++)
		{
			for (int z = Mathf.Max(zMin, -x - yMax); z <= Mathf.Min(zMax, -x - yMin); z++)
			{
				if(Globals.Grid.coordCellLookup.TryGetValue(new HexCoordinates(x, z), out Cell foundCell))
				{
					if (check != null && !check.Invoke(foundCell)) 
						continue;

					grabbedCells.Add(foundCell);
				}
			}
		}

		return grabbedCells;
	}
}
