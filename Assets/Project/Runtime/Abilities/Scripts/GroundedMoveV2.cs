using BOG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "AbilitiesV2/GroundedMove", fileName = "GroundedMove")]
public class GroundedMoveV2 : Ability
{
	[Header("TIMING:")]
	public float stepDuration;
	public float turnDuration;

	[Header("VISUALS:")]
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

		//Debug.LogWarning("Path length: " + path.Length);

		for (int i = 0; i < path.Length; i++)
		{
			Vector2Int from = (i == 0) ? unit.OffsetPos : path[i - 1];
			Vector2Int to = path[i];

			GameObject pathQuad = (GameObject)Instantiate(pathQuadPrefab);
			pathQuad.transform.position = Board.OffsetToWorld(from);

			HexDirectionFT hexDir = from.ToNeighbour(to);
			Vector3 lookDir = hexDir.ToVector();

			pathQuad.transform.rotation = Quaternion.LookRotation(lookDir);

			pathQuads.Add(pathQuad);
		}

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

	public override void HidePreview()
	{
		foreach(var quad in pathQuads)
		{
			Destroy(quad.gameObject);
		}

		pathQuads.Clear();
	}

	public override Queue<UnitCommand> FetchCommandChain(Vector2Int targetCoord, Unit unit)
	{
		//... might not really need these checks...
		if (targetCoord == unit.OffsetPos)
			return null;

		Cell originCell = Board.TryGetCellAtPos(targetCoord);
		if (originCell == null)
			return null;

		Unit foundUnit = Board.GetUnitAtPos(targetCoord);
		if (foundUnit != null && foundUnit.preset != null && !foundUnit.preset.isPassable)
			return null;

		Vector2Int[] path = Board.FindPath(unit.OffsetPos, targetCoord);
		if (path.Length == 0)
			return null;

		Queue<UnitCommand> commands = new Queue<UnitCommand>();

		HexDirectionFT toFirstCellDir = unit.OffsetPos.ToNeighbour(path[0]);

		if(unit.Facing != toFirstCellDir)
		{
			TurnCommandV2 newTurnCommand = new TurnCommandV2(
				unit,
				unit.Facing,
				toFirstCellDir,
				turnDuration
				);

			commands.Enqueue(newTurnCommand);

			string firstTurnLog = $"doing turn from : {unit.Facing} to {toFirstCellDir}";
			Debog.logGameflow(firstTurnLog);
		}

		HexDirectionFT lastFacingDir = toFirstCellDir;
		for (int i = 0; i < path.Length; i++)
		{
			Vector2Int fromCell = i == 0 ? unit.OffsetPos : path[i - 1];
			Vector2Int toCell = path[i];
			HexDirectionFT toNextCellDir = fromCell.ToNeighbour(toCell);
			if(lastFacingDir != toNextCellDir)
			{
				TurnCommandV2 newTurnCommand = new TurnCommandV2(
					unit,
					lastFacingDir,
					toNextCellDir,
					turnDuration
					);

				commands.Enqueue(newTurnCommand);
				lastFacingDir = toNextCellDir;

				string firstTurnLog = $"doing turn from : {lastFacingDir} to {toFirstCellDir}";
				Debog.logGameflow(firstTurnLog);
			}

			var newStepCommand = new MoveCommand(unit, fromCell, toCell, stepDuration);
			commands.Enqueue(newStepCommand);

			string nextLog = $"from: {fromCell} to {toCell}";
			Debog.logGameflow(nextLog);
		}

		return commands;
	}
}
