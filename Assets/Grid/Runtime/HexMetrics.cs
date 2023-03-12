using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
	public const float outerRadius = 2f;
	public const float innerRadius = outerRadius * 0.866025404f;

	public static int CubeDistance(this Cell_OLD from, Cell_OLD to)
	{
		return Mathf.Max(
			Mathf.Abs(from.coords.X - to.coords.X),
			Mathf.Abs(from.coords.Y - to.coords.Y),
			Mathf.Abs(from.coords.Z - to.coords.Z)
			);
	}
}
