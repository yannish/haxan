using BOG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedMoveWithBash : Ability
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

		return path.ToList();
	}

	public override void HidePreview()
	{
		foreach (var quad in pathQuads)
		{
			Destroy(quad.gameObject);
		}

		pathQuads.Clear();
	}

	public override Queue<UnitCommandStep> FetchCommandStepChain(Vector2Int targetCoord, Unit unit)
	{
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


		Queue<UnitCommandStep> commandSteps = new Queue<UnitCommandStep>();

		HexDirectionFT toFirstCellDir = unit.OffsetPos.ToNeighbour(path[0]);
		if (unit.Facing != toFirstCellDir)
		{
			TurnCommand newTurnCommand = new TurnCommand(
				unit,
				unit.Facing,
				toFirstCellDir,
				turnDuration
				);

			var turnCommandStep = new UnitCommandStep(unit, newTurnCommand);

			commandSteps.Enqueue(turnCommandStep);

			string firstTurnLog = $"doing turn from : {unit.Facing} to {toFirstCellDir}";
			Debog.logGameflow(firstTurnLog);
		}

		HexDirectionFT lastFacingDir = toFirstCellDir;
		for (int i = 0; i < path.Length; i++)
		{
			Vector2Int fromCell = i == 0 ? unit.OffsetPos : path[i - 1];
			Vector2Int toCell = path[i];
			HexDirectionFT toNextCellDir = fromCell.ToNeighbour(toCell);
			if (lastFacingDir != toNextCellDir)
			{
				TurnCommand newTurnCommand = new TurnCommand(
					unit,
					lastFacingDir,
					toNextCellDir,
					turnDuration
					);

				var turnCommandStep = new UnitCommandStep(unit, newTurnCommand);

				commandSteps.Enqueue(turnCommandStep);
				lastFacingDir = toNextCellDir;

				string firstTurnLog = $"doing turn from : {lastFacingDir} to {toFirstCellDir}";
				Debog.logGameflow(firstTurnLog);
			}

			var newStepCommand = new MoveCommand(unit, fromCell, toCell, stepDuration);
			var moveCommandStep = new UnitCommandStep(unit, newStepCommand);
			commandSteps.Enqueue(moveCommandStep);

			string nextLog = $"from: {fromCell} to {toCell}";
			Debog.logGameflow(nextLog);
		}

		return commandSteps;
	}

	public override Queue<UnitCommand> FetchCommandChain_OLD(Vector2Int targetCoord, Unit unit)
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

		if (unit.Facing != toFirstCellDir)
		{
			TurnCommand newTurnCommand = new TurnCommand(
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
			if (lastFacingDir != toNextCellDir)
			{
				TurnCommand newTurnCommand = new TurnCommand(
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
