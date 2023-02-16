using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AbilitiesV2/GroundedMove", fileName = "GroundedMove")]
public class GroundedMoveV2 : AbilityV2
{
	public GameObject pathQuadPrefab;

	List<GameObject> pathQuads;

    public override List<Vector2Int> GetValidCoords(Vector2Int origin, Unit unit)
	{
		List<Vector2Int> coordsInRange = Board.GetNavigableTiles(unit).ToList();
		return coordsInRange;
	}

	public override List<Vector2Int> GetAffectedCells(Vector2Int origin, Vector2Int destination, Unit unit)
	{
		Vector2Int[] path = Board.FindPath(origin, destination);

		//string pathName = $"Path{unit.GetInstanceID()}";

		//for (int i = 0; i < path.Length; i++)
		//{
		//	Vector2Int from = (i == 0) ? unit.OffsetPos : path[i - 1];
		//	Vector2Int to = path[i];
		//	GameObject pathQuad = (GameObject)Instantiate(pathQuadPrefab);
		//	pathQuad.name = pathName;
		//	pathQuad.transform.position = Board.OffsetToWorld(from);
		//	// Rotate the path by locating its index in the neighbor
		//	// look-up table
		//	float degrees = 0f;
		//	{
		//		int parity = from.x & 1;
		//		Vector2Int delta = to - from;
		//		int j;
		//		for (j = 0; j < 6; j++)
		//		{
		//			if (Board.neighborLut[parity, j] == delta)
		//				break;
		//		}
		//		degrees = (1 + j) * 60f;
		//	}
		//	pathQuad.transform.rotation = Quaternion.Euler(0, degrees, 0);
		//}

		return path.ToList();
	}

	//public 
}
