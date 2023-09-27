using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitOpRunner : MonoBehaviour
{
	public const int MAX_OPS = 10;

    public IOperable[] allOps = new IOperable[MAX_OPS];

	public Unit dummyUnit;

	private void Start()
	{
		allOps[0] = new UnitMove(dummyUnit, new Vector2Int(2, 3), new Vector2Int(4, 1));
		allOps[1] = new UnitMove(dummyUnit, new Vector2Int(3, 1), new Vector2Int(6, 2));
	}
}
