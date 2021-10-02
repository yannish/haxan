using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoreMath
{
	public static int Max(int x, int y)
	{
		return Mathf.Max(x, y);
	}

	public static int Max(int x, int y, int z)
	{
		// Or inline it as x < y ? (y < z ? z : y) : (x < z ? z : x);
		// Time it before micro-optimizing though!
		return Mathf.Max(x, Mathf.Max(y, z));
	}

	public static int Max(int w, int x, int y, int z)
	{
		return Mathf.Max(w, Mathf.Max(x, Mathf.Max(y, z)));
	}

	public static int Max(params int[] values)
	{
		return Enumerable.Max(values);
	}
}
