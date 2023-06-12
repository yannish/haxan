using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuffAbility : Ability_OLD
{
	public PooledMonoBehaviour pooledPuffPfx;
	public FloatReference turnDuration;


	public override List<Cell_OLD> GetValidMoves(Cell_OLD cell, CharacterFlowController flow)
	{
		List<Cell_OLD> validCells = flow.character.currCell.GetCellsInRadius(
			1,
			cell => !cell.IsBound()
			);

		return validCells;
	}

	public override Queue<CellObjectCommand> FetchCommandChain(
		Cell_OLD targetCell,
		CellObject cellObj,
		FlowController flow
		)
	{
		Queue<CellObjectCommand> newCommands = new Queue<CellObjectCommand>();

		HexDirection initialFacingDir = cellObj.facing;
		HexDirection puffDir = cellObj.currCell.To(targetCell);

		if(cellObj.facing != puffDir)
		{
			TurnCommand newTurnCommand = new TurnCommand(
				cellObj,
				cellObj.facing,
				puffDir,
				turnDuration
				);

			newCommands.Enqueue(newTurnCommand);
		}

		PuffCommand puffCommand = new PuffCommand(
			cellObj,
			initialFacingDir,
			puffDir,
			turnDuration
			);


		Vector3 midway = (cellObj.currCell.transform.position + targetCell.transform.position) * 0.5f;
		Vector3 vertOffset = Vector3.up * 1.2f;

		var newPooledParticle = pooledPuffPfx.GetAndPlay(midway + vertOffset, puffDir.ToVector(), null, false);
		var newPfx = newPooledParticle.GetComponent<ParticleSystem>();

		newPfx.Play();
		newPfx.Pause();
		//newPfx.Sim
		puffCommand.pfx = newPfx;

		newCommands.Enqueue(puffCommand);

		return null;
	}
}
