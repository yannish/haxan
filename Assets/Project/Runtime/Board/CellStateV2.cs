using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum CellSurfaceFlags
{
	DAMP = 1 << 1,
	DUSTY = 1 << 2,
	MOLDY = 1 << 3,
	CRACKED = 1 << 4
}

public class CellStateV2
{
    public CellSurfaceFlags surfaceFlags;
}
