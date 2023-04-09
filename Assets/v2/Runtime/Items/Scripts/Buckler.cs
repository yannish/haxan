using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Buckler", fileName = "Buckler")]
public class Buckler : Item
{
	[Header("VISUALS:")]
	public PooledIndicator bashMarker;
	//public PooleunbashableMarker;

	//... preview the what's going to be bashed:
	public override void ShowPathReaction(Vector2Int origin, List<Vector2Int> path)
	{
		if(path.Count < 1)
		{
			Debug.LogWarning("trying to preview too short of a path");
		}

		Debug.LogWarning("buckler reacting to path");

		foreach(var indicator in previewedIndicators)
		{
			indicator.Hide();
		}

		previewedIndicators.Clear();

		Vector2Int originCoord = origin;
		Vector2Int firstPathCoord = path[0];

		PreviewMove(originCoord, firstPathCoord);

		for (int i = 0; i < path.Count - 1; i++)
		{
			Vector2Int fromCoord = path[i];
			Vector2Int toCoord = path[i + 1];

			PreviewMove(fromCoord, toCoord);
		}
	}



	//... Find all units that are adjacent to both the start & end coord:
	List<PooledIndicator> previewedIndicators = new List<PooledIndicator>();
	void PreviewMove(Vector2Int fromCoord, Vector2Int toCoord)
	{
		List<Unit> foundFromUnits = Board.GetNeighbouringUnits(fromCoord);
		List<Unit> foundToUnits = Board.GetNeighbouringUnits(toCoord);

		List<Unit> sharedNeighbours = new List<Unit>();
		foreach (var fromUnit in foundFromUnits)
		{
			foreach (var toUnit in foundToUnits)
			{
				if (fromUnit == toUnit)
				{
					sharedNeighbours.Add(fromUnit);
				}
			}
		}

		Debug.LogWarning("shared neighbours: " + sharedNeighbours.Count);

		foreach(var neighbour in sharedNeighbours)
		{
			Vector3 neighbourWorldPos = Board.OffsetToWorld(neighbour.OffsetPos);
			Vector3 fromWorldPos = Board.OffsetToWorld(fromCoord);
			Vector3 fromCoordToNeighbourDir = fromWorldPos.To(neighbourWorldPos);

			if (neighbour.preset != null)
			{
				var bashMarkerInstance = bashMarker.GetAndPlay(neighbourWorldPos, fromCoordToNeighbourDir);
				if (neighbour.preset.knockResistance != KnockResistance.IMMOVABLE)
				{
					bashMarker.SetInvalid();
				}
				previewedIndicators.Add(bashMarkerInstance);
			}
		}
	}

	public override void HidePathReaction()
	{
		Debug.LogWarning("hiding buckler reaction to path");

		foreach (var indicator in previewedIndicators)
			indicator.Hide();

		previewedIndicators.Clear();
	}

	//... actually add the commands to step:
	public override void RespondToCommandStep(Unit unit, ref UnitCommandStep commandStep)
	{
		if(commandStep.instigatingCommand is MoveCommand)
		{
			MoveCommand moveCommand = commandStep.instigatingCommand as MoveCommand;

			List<Unit> foundFromUnits = Board.GetNeighbouringUnits(moveCommand.fromCoord);
			List<Unit> foundToUnits = Board.GetNeighbouringUnits(moveCommand.toCoord);

			List<Unit> sharedNeighbourds = new List<Unit>();
			foreach (var fromUnit in foundFromUnits)
			{
				foreach(var toUnit in foundToUnits)
				{
					if(fromUnit == toUnit)
					{
						sharedNeighbourds.Add(fromUnit);
					}
				}
			}

			
		}
	}
}
